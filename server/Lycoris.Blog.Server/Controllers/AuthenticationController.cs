﻿using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Application.AppService.Authentication;
using Lycoris.Blog.Application.AppService.Authentication.Dtos;
using Lycoris.Blog.Application.Cached.AuthenticationCache;
using Lycoris.Blog.Application.Cached.EmailCache;
using Lycoris.Blog.Application.Cached.EmailCache.Dtos;
using Lycoris.Blog.Core.Email;
using Lycoris.Blog.Core.Email.DataModel;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Application.Swaggers;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Authentication;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 身份验证
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Authentication")]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationAppService _authentication;
        private readonly IAuthenticationCacheService _cache;
        private readonly Lazy<IEmailAppService> _email;
        private readonly Lazy<IEmailCacheService> _emailCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authentication"></param>
        /// <param name="cache"></param>
        /// <param name="email"></param>
        /// <param name="emailCache"></param>
        public AuthenticationController(IAuthenticationAppService authentication, IAuthenticationCacheService cache, Lazy<IEmailAppService> email, Lazy<IEmailCacheService> emailCache)
        {
            _authentication = authentication;
            _cache = cache;
            _email = email;
            _emailCache = emailCache;
        }

        /// <summary>
        /// 帐号密码验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login/Validate")]
        [ExcludeSwaggerHeader, Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginValidateViewModel>> LoginValidate([FromBody] LoginValidateInput input)
        {
            var dto = await _authentication.LoginValidateAsync(input.Email!, input.Password!);
            var oathCode = RandomHelper.GetRandomLetterStringLower(32);

            // 缓存记录
            await _cache.SetLoginOathCodeAsync(input.Email!, oathCode, dto);

            return Success(new LoginValidateViewModel(oathCode, dto.GoogleAuthentication));
        }

        #region ============ 前台 ============
        /// <summary>
        /// 授权码登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ExcludeSwaggerHeader, Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> Login([FromBody] LoginInput input)
        {
            var cache = await _cache.GetLoginOathCodeAsync(input.Email!) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, "oathcode is expired");
            if (cache.OathCode != input.OathCode)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var dto = await _authentication.LoginAsync(cache.Value!, input.Remember ?? false, false);

            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Logout()
        {
            await _authentication.LogoutAsync(false);
            return Success();
        }

        /// <summary>
        /// 访问令牌刷新
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Refresh/Token")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<RefreshTokenViewModel>> RefreshToken([FromBody] RefreshTokenInput input)
        {
            var dto = await _authentication.RefreshTokenAsync(input.RefreshToken!, false);
            return Success(new RefreshTokenViewModel(dto.Token));
        }

        /// <summary>
        /// 邮箱注册验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        [HttpPost("Captcha/Register")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<RegisterCaptchaViewModel>> RegisterCaptcha([FromBody] RegisterCaptchaInputInput input)
        {
            var cache = await _emailCache.Value.GetEmailCaptchaAsync(input.Email!, EmailTypeEnum.Register);
            if (cache != null && cache.CaptchaExpiredTime!.Value > DateTime.Now)
            {
                var times = (int)Math.Ceiling((cache.CaptchaExpiredTime!.Value - DateTime.Now).TotalSeconds);
                return new DataOutput<RegisterCaptchaViewModel>()
                {
                    ResCode = ResCodeEnum.Friendly,
                    ResMsg = $"请{times}秒后再试",
                    Data = new RegisterCaptchaViewModel(times)
                };
            }

            var isUse = await _authentication.CheckEmailUseAsync(input.Email!);
            if (isUse)
                throw new FriendlyException("邮箱已被注册");

            var data = new SendEmailCaptchaDataModel()
            {
                EmailAddress = input.Email!,
                Code = RandomHelper.GetRandomString(6).ToUpper(),
                Action = "注册",
                ExpireTime = 5
            };

            // 发送邮件
            await _email.Value.SendEmailCaptchaAsync(data);

            // 验证码时间
            await _emailCache.Value.SetEmailCaptchaAsync(input.Email!, EmailTypeEnum.Register, new EmailCaptchaCacheDto(data.Code, data.ExpireTime!.Value));

            return Success(new RegisterCaptchaViewModel());
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Register([FromBody] RegisterInput input)
        {
            if (input.Captcha!.Length != 6)
                throw new FriendlyException("验证码错误");

            var cache = await _emailCache.Value.GetEmailCaptchaAsync(input.Email!, EmailTypeEnum.Register);
            if (cache == null)
                throw new FriendlyException("验证码已过期");
            else if (cache.Code != input.Captcha)
                throw new FriendlyException("验证码错误");

            // 
            await _authentication.RegisterAsync(input.ToMap<RegisterDto>());

            return Success();
        }

        /// <summary>
        /// 管理后台登录地址
        /// </summary>
        /// <returns></returns>
        [HttpPost("Admin"), WebAuthentication(IsRequired = true)]
        public async Task<DataOutput<AdminViewModel>> Admin()
        {
            if (!CurrentRequest!.User!.IsAdmin)
                return Success(new AdminViewModel());

            var path = await _authentication.GetAdminPathAsync();
            return Success(new AdminViewModel(path));
        }
        #endregion

        #region ============ 后台 ============
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("SSO/Login")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> DashboardSSOLogin()
        {
            var dto = await _authentication.DashboardSSOLoginAsync();
            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 授权码登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("Dashboard/Login")]
        [ExcludeSwaggerHeader, Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> DashboardLogin([FromBody] LoginInput input)
        {
            var cache = await _cache.GetLoginOathCodeAsync(input.Email!) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, "oathcode is expired");
            if (cache.OathCode != input.OathCode)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var dto = await _authentication.LoginAsync(cache.Value!, input.Remember ?? false, true);

            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("Dashboard/Logout")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> DashboardLogout()
        {
            await _authentication.LogoutAsync(true);
            return Success();
        }

        /// <summary>
        /// 访问令牌刷新
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Dashboard/Refresh/Token")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<RefreshTokenViewModel>> DashboardRefreshToken([FromBody] RefreshTokenInput input)
        {
            var dto = await _authentication.RefreshTokenAsync(input.RefreshToken!, true);
            return Success(new RefreshTokenViewModel(dto.Token));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Dashboard/Screen/UnLock")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> UnLock([FromBody] UnLockInput input)
        {
            await _authentication.ScreenUnLockAsync(input.Password!);
            return Success();
        }
        #endregion
    }
}
