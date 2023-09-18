using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Application.AppService.Authentication.Dtos;
using Lycoris.Blog.Application.AppService.LoginTokens;
using Lycoris.Blog.Application.Cached.AuthenticationCache;
using Lycoris.Blog.Application.Cached.AuthenticationCache.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Common.Impl;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Lycoris.Blog.Application.AppService.Authentication.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true, EnableInterceptor = true)]
    public class AuthenticationAppService : ApplicationBaseService, IAuthenticationAppService
    {
        private readonly IRepository<User, long> _user;
        private readonly IRepository<LoginToken, int> _loginToken;
        private readonly Lazy<ILoginTokenAppService> _loginTokenService;
        private readonly Lazy<IRepository<UserLink, long>> _userLink;
        private readonly Lazy<IAuthenticationCacheService> _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginToken"></param>
        /// <param name="loginTokenService"></param>
        /// <param name="cache"></param>
        public AuthenticationAppService(IRepository<User, long> user,
                                        IRepository<LoginToken, int> loginToken,
                                        Lazy<ILoginTokenAppService> loginTokenService,
                                        Lazy<IRepository<UserLink, long>> userLink,
                                        Lazy<IAuthenticationCacheService> cache)
        {
            _user = user;
            _loginToken = loginToken;
            _loginTokenService = loginTokenService;
            _userLink = userLink;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<LoginValidateDto> LoginValidateAsync(string email, string password)
        {
            email = email.ToLower();
            var data = await _user.GetAsync(x => x.Email == email) ?? throw new FriendlyException("帐号或密码错误", $"not find user with email:{email}");

            if (data.Password != SqlPasswrodConverter.Encrypt(password))
                throw new FriendlyException("帐号或密码错误", $"email:{email} password verification failed");
            else if (data.Status == UserStatusEnum.Defalut)
                throw new FriendlyException("帐号还未通过审核", $"email:{email} has not been approved");

            return data.ToMap<LoginValidateDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remember"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        public async Task<LoginDto> LoginAsync(LoginValidateDto input, bool remember, bool isManagement)
        {
            var dto = input.ToMap<LoginDto>();

            var data = await _loginToken.GetAll().Where(x => x.UserId == input.Id!.Value).Where(x => x.IsManagement == isManagement).SingleOrDefaultAsync() ?? new LoginToken() { UserId = input.Id!.Value, IsManagement = isManagement };

            if (data.Id == input.Id!.Value)
            {
                // 数据库如果记录的缓存未过期，则删除就缓存
                if (data.TokenExpireTime > DateTime.Now)
                    await _cache.Value.SetLogoutStateAsync(data.Token);
            }

            // 生成访问令牌
            data.TokenExpireTime = DateTime.Now.AddSeconds(30);
            data.Token = _loginTokenService.Value.GenereateToken(input.Id.Value, data.TokenExpireTime);

            // 生成刷新令牌
            data.RefreshTokenExpireTime = DateTime.Now.AddDays(remember ? 15 : 1);
            data.RefreshToken = _loginTokenService.Value.GenereateToken(input.Id.Value, data.RefreshTokenExpireTime);

            // 插入数据库
            await _loginToken.CreateOrUpdateAsync(data);

            dto.Token = data.Token;
            dto.TokenExpireTime = data.TokenExpireTime;
            dto.RefreshToken = data.RefreshToken;

            // 设置登录令牌缓存
            await _cache.Value.SetLoginStateAsync(dto);

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task LogoutAsync(bool isManagement)
        {
            if (!this.LoginState || this.CurrentRequest.Token.IsNullOrEmpty())
                return;

            var (userId, expiredTime, isAdmin) = _loginTokenService.Value.AnalyzeToken(CurrentRequest.Token);

            if (!userId.HasValue || (expiredTime.HasValue && expiredTime.Value <= DateTime.Now))
                return;

            var data = await _loginToken.GetAll().Where(x => x.UserId == userId.Value).Where(x => x.IsManagement == isManagement).SingleOrDefaultAsync();
            if (data == null)
                return;

            if (data.Token != CurrentRequest.Token)
                throw new FriendlyException("退出登录失败", $"token not match,database:{data.Token},request:{CurrentRequest.Token}");

            await _cache.Value.SetLogoutStateAsync(data.Token);

            data.TokenExpireTime = DateTime.Now.AddSeconds(-1);
            data.RefreshTokenExpireTime = DateTime.Now.AddSeconds(-1);
            await _loginToken.UpdateFieIdsAsync(data, x => x.TokenExpireTime, x => x.RefreshTokenExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        /// <exception cref="HttpStatusException"></exception>
        public async Task<LoginDto> RefreshTokenAsync(string refreshToken, bool isManagement)
        {
            var (userId, expiredTime, isAdmin) = _loginTokenService.Value.AnalyzeToken(refreshToken);
            if (!userId.HasValue || !expiredTime.HasValue)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "token parsing failed, does not comply with system generation logic");

            var data = await _loginToken.GetAll().Where(x => x.UserId == userId.Value).Where(x => x.IsManagement == isManagement).SingleOrDefaultAsync()
                ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"not find token data with user id:{userId}");

            if (data.RefreshToken != refreshToken || data.RefreshTokenExpireTime <= DateTime.Now)
                throw new HttpStatusException(HttpStatusCode.Unauthorized, $"refresh token not match,database:{data.RefreshToken},request:{refreshToken}");

            var cache = await _cache.Value.GetLoginStateAsync(data.Token);
            if (cache == null)
            {
                cache = await _user.GetSelectAsync(userId!.Value, x => new LoginUserCacheDto()
                {
                    Id = x.Id,
                    NickName = x.NickName,
                    Avatar = x.Avatar,
                    IsAdmin = x.IsAdmin
                });
            }
            else
                await _cache.Value.SetLogoutStateAsync(data.Token);

            // 生成访问令牌
            data.TokenExpireTime = DateTime.Now.AddMinutes(31);
            data.Token = _loginTokenService.Value.GenereateToken(userId.Value, data.TokenExpireTime);

            // 刷新刷新令牌的过期时间
            if (data.RefreshTokenExpireTime.AddHours(-12) < DateTime.Now)
                data.RefreshTokenExpireTime = DateTime.Now.AddDays(1);

            await _loginToken.UpdateFieIdsAsync(data, x => x.Token, x => x.TokenExpireTime, x => x.RefreshTokenExpireTime);

            cache!.TokenExpireTime = data.TokenExpireTime;

            // 写入缓存
            await _cache.Value.SetLoginStateAsync(data.Token, cache);

            return new LoginDto()
            {
                Id = cache.Id,
                NickName = cache.NickName,
                Avatar = cache.Avatar,
                IsAdmin = cache.IsAdmin,
                Token = data.Token,
                TokenExpireTime = data.TokenExpireTime,
                RefreshToken = data.RefreshToken
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpStatusException"></exception>
        public async Task<LoginDto> DashboardSSOLoginAsync()
        {
            var user = await _user.GetAsync(CurrentUser!.Id) ?? throw new HttpStatusException(HttpStatusCode.Unauthorized, "");

            var data = await _loginToken.GetAsync(x => x.UserId == user!.Id && x.IsManagement == true);
            if (data == null || data.RefreshTokenExpireTime <= DateTime.Now)
                return await LoginAsync(user.ToMap<LoginValidateDto>(), data?.Remember ?? true, true);

            if (data.TokenExpireTime <= DateTime.Now)
                return await RefreshTokenAsync(data.RefreshToken, true);

            return new LoginDto()
            {
                Id = user.Id,
                NickName = user.NickName,
                Avatar = user.Avatar,
                IsAdmin = user.IsAdmin,
                Token = data.Token,
                TokenExpireTime = data.TokenExpireTime,
                RefreshToken = data.RefreshToken
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        public async Task<LoginUserCacheDto?> GetLoginUserAsync(string token, bool isManagement)
        {
            var cache = await _cache.Value.GetLoginStateAsync(token);
            if (cache != null && cache.TokenExpireTime > DateTime.Now)
                return cache;

            var (userId, expitedTime, isAdmin) = _loginTokenService.Value.AnalyzeToken(token);
            if (userId == null || userId.Value == 0)
                return null;
            else if (expitedTime == null)
                return null;
            else if (expitedTime <= DateTime.Now)
                throw new OutputException(ResCodeEnum.TokenExpired);

            var user = await _user.GetAsync(userId!.Value);
            if (user == null)
            {
                return null;
            }


            var data = await _loginToken.GetAll().Where(x => x.UserId == userId!.Value).Where(x => x.IsManagement == isManagement).SingleOrDefaultAsync();
            if (data == null)
            {

                return null;
            }

            if (data.Token != token)
            {

                return null;
            }


            cache = new LoginUserCacheDto()
            {
                Id = user.Id,
                NickName = user.NickName,
                Avatar = user.Avatar,
                IsAdmin = isAdmin,
                TokenExpireTime = data.TokenExpireTime
            };

            await _cache.Value.SetLoginStateAsync(token, cache);

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> CheckEmailUseAsync(string email)
        {
            email = email.ToLower();
            return await _user.GetAll().Where(x => x.Email == email).AnyAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RegisterAsync(RegisterDto input)
        {
            input.Email = input.Email.ToLower();
            if (await CheckEmailUseAsync(input.Email))
                throw new FriendlyException("邮箱已被注册");

            var data = await _user.CreateAsync(new User()
            {
                Email = input.Email,
                NickName = RandomHelper.GetRandomNickName(),
                Avatar = "/images/404.png",
                Password = input.Password,
                ShowOnlineStatus = true,
                CreateTime = DateTime.Now,
                GoogleAuthentication = false,
                IsAdmin = false
            });

            await _userLink.Value.CreateAsync(new UserLink() { Id = data.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAdminPathAsync()
        {
            var config = await this.ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSettings);
            return config?.AdminPath ?? "";
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ChangePasswordAsync(ChangePasswordDto input)
        {
            if (input.Password.IsNullOrEmpty())
                throw new HttpStatusException(HttpStatusCode.BadRequest, "修改密码为空");

            if (input.Password == input.OldPassword)
                return;

            var data = await _user.GetAsync(CurrentUser.Id);
            if (data!.Password != SqlPasswrodConverter.Encrypt(input.Password!))
            {
                // 记录错误次数
                throw new FriendlyException("密码错误");
            }

            data.Password = input.Password!;

            await _user.UpdateFieIdsAsync(data, x => x.Password);
        }
    }
}
