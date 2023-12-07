using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Application.AppServices.LoginTokens;
using Lycoris.Blog.Application.Cached.Authentication;
using Lycoris.Blog.Application.Cached.Authentication.Models;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Lycoris.Blog.EntityFrameworkCore.Common.Impl;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Lycoris.Blog.Application.AppServices.Authentication.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true, EnableInterceptor = true)]
    public class AuthenticationAppService : ApplicationBaseService, IAuthenticationAppService
    {
        /// <summary>
        /// 分钟
        /// </summary>
        const int TokenExpireTime = 10;
        /// <summary>
        /// 天数
        /// </summary>
        const int RefreshTokenExpireTime = 14;

        private readonly IRepository<User, long> _user;
        private readonly IRepository<LoginToken, int> _loginToken;
        private readonly Lazy<ILoginTokenAppService> _loginTokenService;
        private readonly Lazy<IRepository<UserLink, long>> _userLink;
        private readonly Lazy<IAuthenticationCacheService> _cache;
        private readonly Lazy<IScheduleQueueCacheService> _scheduleQueueCache;

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
                                        Lazy<IAuthenticationCacheService> cache,
                                        Lazy<IScheduleQueueCacheService> scheduleQueueCache)
        {
            _user = user;
            _loginToken = loginToken;
            _loginTokenService = loginTokenService;
            _userLink = userLink;
            _cache = cache;
            _scheduleQueueCache = scheduleQueueCache;
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
            {
                _scheduleQueueCache.Value.Enqueue(ScheduleTypeEnum.LoginRecord, new LoginRecordQueueModel(data.Id, CurrentRequest, false, "密码核验失败"));
                throw new FriendlyException("帐号或密码错误", $"email:{email} password verification failed");
            }
            else if (data.Status == UserStatusEnum.Defalut)
                throw new FriendlyException("帐号还未通过审核", $"email:{email} has not been approved");
            else if (data.Status == UserStatusEnum.Black)
                throw new FriendlyException("您的账号存在异常，请联系站长处理", $"email:{email} user status is black");
            else if (data.Status == UserStatusEnum.Cancellation)
                throw new FriendlyException("您的账号已被注销，请联系站长处理", $"email:{email} user status is cancellation");

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

            var data = await _loginToken.GetAll()
                                        .Where(x => x.UserId == input.Id!.Value)
                                        .Where(x => x.IsManagement == isManagement)
                                        .SingleOrDefaultAsync() ?? new LoginToken() { UserId = input.Id!.Value, IsManagement = isManagement };

            if (data.UserId == input.Id!.Value)
            {
                // 数据库如果记录的缓存未过期，则删除就缓存
                if (data.TokenExpireTime > DateTime.Now)
                    _cache.Value.SetLogoutState(data.Token);
            }

            // 生成访问令牌
            data.TokenExpireTime = DateTime.Now.AddMinutes(TokenExpireTime);
            data.Token = _loginTokenService.Value.GenereateToken(input.Id.Value, data.TokenExpireTime, dto.IsAdmin ?? false);

            // 生成刷新令牌
            data.RefreshTokenExpireTime = DateTime.Now.AddDays(remember ? RefreshTokenExpireTime : 1);
            data.RefreshToken = _loginTokenService.Value.GenereateToken(input.Id.Value, data.RefreshTokenExpireTime, dto.IsAdmin ?? false);

            // 插入数据库
            await _loginToken.CreateOrUpdateAsync(data);

            dto.Token = data.Token;
            dto.TokenExpireTime = data.TokenExpireTime;
            dto.RefreshToken = data.RefreshToken;
            dto.RefreshTokenExpireTime = data.RefreshTokenExpireTime;

            // 设置登录令牌缓存
            _cache.Value.SetLoginState(dto.Token, dto.ToMap<LoginUserCacheModel>());

            // 登录记录
            _scheduleQueueCache.Value.Enqueue(ScheduleTypeEnum.LoginRecord, new LoginRecordQueueModel(input.Id!.Value, CurrentRequest));

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
            if (!LoginState || CurrentRequest.Token.IsNullOrEmpty())
                return;

            var (userId, expiredTime, isAdmin) = _loginTokenService.Value.AnalyzeToken(CurrentRequest.Token);

            if (!userId.HasValue || expiredTime.HasValue && expiredTime.Value <= DateTime.Now)
                return;

            var data = await _loginToken.GetAll().Where(x => x.UserId == userId.Value).Where(x => x.IsManagement == isManagement).SingleOrDefaultAsync();
            if (data == null)
                return;

            if (data.Token != CurrentRequest.Token)
                throw new FriendlyException("退出登录失败", $"token not match,database:{data.Token},request:{CurrentRequest.Token}");

            _cache.Value.SetLogoutState(data.Token);

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

            var cache = _cache.Value.GetLoginState(data.Token);
            if (cache == null)
            {
                cache = await _user.GetSelectAsync(userId!.Value, x => new LoginUserCacheModel()
                {
                    Id = x.Id,
                    NickName = x.NickName,
                    Avatar = x.Avatar,
                    IsAdmin = x.IsAdmin
                });
            }
            else
                _cache.Value.SetLogoutState(data.Token);

            // 生成访问令牌
            data.TokenExpireTime = DateTime.Now.AddMinutes(TokenExpireTime);
            data.Token = _loginTokenService.Value.GenereateToken(userId.Value, data.TokenExpireTime, cache!.IsAdmin ?? false);

            // 刷新刷新令牌的过期时间
            if (data.RefreshTokenExpireTime.AddHours(-12) < DateTime.Now)
                data.RefreshTokenExpireTime = data.Remember ? DateTime.Now.AddDays(RefreshTokenExpireTime) : DateTime.Now.AddDays(1);

            await _loginToken.UpdateFieIdsAsync(data, x => x.Token, x => x.TokenExpireTime, x => x.RefreshTokenExpireTime);

            cache!.TokenExpireTime = data.TokenExpireTime;

            // 写入缓存
            _cache.Value.SetLoginState(data.Token, cache);

            return new LoginDto()
            {
                Id = cache.Id,
                NickName = cache.NickName,
                Avatar = cache.Avatar,
                IsAdmin = cache.IsAdmin,
                Token = data.Token,
                TokenExpireTime = data.TokenExpireTime,
                RefreshToken = data.RefreshToken,
                RefreshTokenExpireTime = data.RefreshTokenExpireTime
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpStatusException"></exception>
        public async Task<LoginDto> DashboardSSOLoginAsync()
        {
            var user = await _user.GetAsync(CurrentUser!.Id) ?? throw new HttpStatusException(HttpStatusCode.Unauthorized, $"can not find user by id：{CurrentUser!.Id}");

            LoginDto? dto = null;

            var data = await _loginToken.GetAsync(x => x.UserId == user!.Id && x.IsManagement == true);

            if (data == null || data.RefreshTokenExpireTime <= DateTime.Now)
                dto = await LoginAsync(user.ToMap<LoginValidateDto>(), data?.Remember ?? true, true);
            else if (data.TokenExpireTime <= DateTime.Now)
                dto = await RefreshTokenAsync(data.RefreshToken, true);
            else
            {
                dto = new LoginDto()
                {
                    Id = user.Id,
                    NickName = user.NickName,
                    Avatar = user.Avatar,
                    IsAdmin = user.IsAdmin,
                    Token = data!.Token,
                    TokenExpireTime = data.TokenExpireTime,
                    RefreshToken = data.RefreshToken
                };
            }

            // 登录记录
            _scheduleQueueCache.Value.Enqueue(ScheduleTypeEnum.LoginRecord, new LoginRecordQueueModel(CurrentUser.Id, CurrentRequest, "SSO登录"));

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        public async Task<LoginUserCacheModel?> GetLoginUserAsync(string token, bool isManagement)
        {
            var cache = _cache.Value.GetLoginState(token);
            if (cache != null && cache.TokenExpireTime > DateTime.Now)
                return cache;

            var (userId, expitedTime, isAdmin) = _loginTokenService.Value.AnalyzeToken(token);
            if (userId == null || userId.Value == 0)
                return null;
            else if (expitedTime == null)
                return null;
            else if (expitedTime <= DateTime.Now)
                throw new OutputException(ResCodeEnum.TokenExpired);

            var user = await _user.GetAsync(userId!.Value) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"can not find user by id:{userId}");

            var data = await _loginToken.GetAll().Where(x => x.UserId == userId!.Value).Where(x => x.IsManagement == isManagement).SingleOrDefaultAsync()
                ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"can not find login token by user id:{userId}");

            if (data.Token != token)
                throw new HttpStatusException(HttpStatusCode.Unauthorized, $"login token:{token} can not match server token:{data.Token}");

            cache = new LoginUserCacheModel()
            {
                Id = user.Id,
                NickName = user.NickName,
                Avatar = user.Avatar,
                IsAdmin = isAdmin,
                TokenExpireTime = data.TokenExpireTime
            };

            _cache.Value.SetLoginState(token, cache);

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> CheckEmailUseAsync(string email, long? id = null)
        {
            email = email.ToLower();
            return await _user.GetAll().Where(x => x.Email == email).WhereIf(id.HasValue, x => x.Id != id).AnyAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Transactional]
        public async Task RegisterAsync(RegisterDto input)
        {
            input.Email = input.Email.ToLower();
            if (await CheckEmailUseAsync(input.Email))
                throw new FriendlyException("邮箱已被注册");

            var data = new User()
            {
                Email = input.Email,
                NickName = $"程序猿{RandomHelper.GetRandomString(4)}",
                Password = input.Password,
                Status = UserStatusEnum.Audited,
                ShowOnlineStatus = true,
                CreateTime = DateTime.Now,
                GoogleAuthentication = false,
                IsAdmin = false
            };

            if (data.Email.EndsWith("@qq.com"))
                data.Avatar = $"https://q2.qlogo.cn/headimg_dl?dst_uin={data.Email}&spec=100";
            else
            {
                var setting = await this.ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebStatistics);
                data.Avatar = setting!.DefaultAvatar;
            }

            data = await _user.CreateAsync(data);

            await _userLink.Value.CreateAsync(new UserLink() { Id = data.Id });

            _scheduleQueueCache.Value.Enqueue(ScheduleTypeEnum.WebStatistics, new WebStatisticsQueueModel() { User = 1 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAdminPathAsync()
        {
            var config = await ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSetting);
            return config?.AdminPath ?? "";
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<LoginDto> ChangePasswordAsync(ChangePasswordDto input)
        {
            if (input.Password.IsNullOrEmpty())
                throw new HttpStatusException(HttpStatusCode.BadRequest, "修改密码为空");

            var data = await _user.GetAsync(CurrentUser.Id) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"can not find login user by id:{CurrentUser.Id}");

            if (data.Password == SqlPasswrodConverter.Encrypt(input.OldPassword!))
                throw new FriendlyException("与原密码相同不需要修改");

            if (data.Password != SqlPasswrodConverter.Encrypt(input.OldPassword!))
            {
                // 记录错误次数
                throw new FriendlyException("密码错误");
            }

            data.Password = input.Password!;

            await _user.UpdateFieIdsAsync(data, x => x.Password);

            return await LoginAsync(new LoginValidateDto(data.Id), true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task ScreenUnLockAsync(string password)
        {
            var user = await _user.GetAsync(CurrentUser.Id) ?? throw new HttpStatusException(HttpStatusCode.Unauthorized, $"can not find user by user id:{CurrentUser.Id}");

            if (user.Password != SqlPasswrodConverter.Encrypt(password))
                throw new FriendlyException("密码错误");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Transactional]
        public async Task ChangeEmailAsync(string email)
        {
            var user = await _user.GetAsync(CurrentUser.Id) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"can not find user by id:{CurrentUser.Id}");

            if (user.IsAdmin)
                throw new FriendlyException("超级管理员帐号不允许修改");

            user.Email = email;
            await _user.UpdateFieIdsAsync(user, x => x.Email);

            var data = await _loginToken.GetAll().Where(x => x.UserId == CurrentUser.Id).Where(x => x.IsManagement == false).SingleOrDefaultAsync();
            if (data != null)
            {
                _cache.Value.SetLogoutState(data.Token);

                data.TokenExpireTime = DateTime.Now.AddSeconds(-1);
                data.RefreshTokenExpireTime = DateTime.Now.AddSeconds(-1);
                await _loginToken.UpdateFieIdsAsync(data, x => x.TokenExpireTime, x => x.RefreshTokenExpireTime);
            }
        }
    }
}
