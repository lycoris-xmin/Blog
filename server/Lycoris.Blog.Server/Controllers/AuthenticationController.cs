using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Authentication;
using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Application.Cached.Authentication;
using Lycoris.Blog.Application.Cached.Email;
using Lycoris.Blog.Application.Cached.Email.Models;
using Lycoris.Blog.Core.Email;
using Lycoris.Blog.Core.Email.DataModel;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Authentication;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 身份验证
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Authentication")]
    public class AuthenticationController : BaseApiController
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
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginValidateViewModel>> LoginValidate([FromBody] LoginValidateInput input)
        {
            var dto = await _authentication.LoginValidateAsync(input.Email!, input.Password!);
            var oathCode = RandomHelper.GetRandomLetterStringLower(32);

            // 缓存记录
            _cache.SetLoginOathCode(input.Email!, oathCode, dto);

            return Success(new LoginValidateViewModel(oathCode, dto.GoogleAuthentication));
        }

        #region ============ 前台 ============
        /// <summary>
        /// 授权码登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> Login([FromBody] LoginInput input)
        {
            var cache = _cache.GetLoginOathCode(input.Email!) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, "oathcode is expired");
            if (cache.OathCode != input.OathCode)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var dto = await _authentication.LoginAsync(cache.Value!.ToMap<LoginValidateDto>(), input.Remember ?? false, false);

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
        public async Task<DataOutput<LoginViewModel>> RefreshToken([FromBody] RefreshTokenInput input)
        {
            var dto = await _authentication.RefreshTokenAsync(input.RefreshToken!, false);
            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 邮箱注册验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        [HttpPost("Captcha/Register")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<EmailCaptchaViewModel>> RegisterCaptcha([FromBody] RegisterCaptchaInputInput input)
        {
            var cache = _emailCache.Value.GetEmailCaptcha(input.Email!, EmailTypeEnum.Register);
            if (cache != null && cache.CaptchaExpiredTime!.Value > DateTime.Now)
            {
                var times = (int)Math.Ceiling((cache.CaptchaExpiredTime!.Value - DateTime.Now).TotalSeconds);
                return new DataOutput<EmailCaptchaViewModel>()
                {
                    ResCode = ResCodeEnum.Friendly,
                    ResMsg = $"请{times}秒后再试",
                    Data = new EmailCaptchaViewModel(times)
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
            _emailCache.Value.SetEmailCaptcha(input.Email!, EmailTypeEnum.Register, new EmailCaptchaCacheModel(data.Code, data.ExpireTime!.Value));

            return Success(new EmailCaptchaViewModel());
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

            var cache = _emailCache.Value.GetEmailCaptcha(input.Email!, EmailTypeEnum.Register);
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
        [HttpPost("Admin")]
        [WebAuthentication(IsRequired = true)]
        [Produces("application/json")]
        public async Task<DataOutput<AdminViewModel>> Admin()
        {
            if (!CurrentRequest!.User!.IsAdmin)
                return Success(new AdminViewModel());

            var path = await _authentication.GetAdminPathAsync();
            return Success(new AdminViewModel(path));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> ChangePassword([FromBody] ChangePasswordInput input)
        {
            var data = input.ToMap<ChangePasswordDto>();
            var dto = await _authentication.ChangePasswordAsync(data);
            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 修改邮箱验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Change/Email/Captcha")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<EmailCaptchaViewModel>> ChangeEmailCaptchaCode([FromBody] ChangeEmailCaptchaCodeInput input)
        {
            var cache = _emailCache.Value.GetEmailCaptcha(input.Email!, EmailTypeEnum.ChangeEmail);
            if (cache != null && cache.CaptchaExpiredTime!.Value > DateTime.Now)
            {
                var times = (int)Math.Ceiling((cache.CaptchaExpiredTime!.Value - DateTime.Now).TotalSeconds);
                return new DataOutput<EmailCaptchaViewModel>()
                {
                    ResCode = ResCodeEnum.Friendly,
                    ResMsg = $"请{times}秒后再试",
                    Data = new EmailCaptchaViewModel(times)
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
            _emailCache.Value.SetEmailCaptcha(input.Email!, EmailTypeEnum.ChangeEmail, new EmailCaptchaCacheModel(data.Code, data.ExpireTime!.Value));

            return Success(new EmailCaptchaViewModel());
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Change/Email")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> ChangeEmail([FromBody] ChangeEmailInput input)
        {
            if (input.Captcha!.Length != 6)
                throw new FriendlyException("验证码错误");

            var cache = _emailCache.Value.GetEmailCaptcha(input.Email!, EmailTypeEnum.ChangeEmail);
            if (cache == null)
                throw new FriendlyException("验证码已过期");
            else if (cache.Code != input.Captcha)
                throw new FriendlyException("验证码错误");

            await _authentication.ChangeEmailAsync(input.Email!);
            return Success();
        }

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <returns></returns>
        [HttpPost("Cancellation")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<UserCancellationViewModel>> UserCancellation()
        {
            var time = await _authentication.UserCancellationAsync();
            return Success(new UserCancellationViewModel(time));
        }

        /// <summary>
        /// 停止注销
        /// </summary>
        /// <returns></returns>
        [HttpPost("Cancellation/Stop")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> StopUserCancellation()
        {
            await _authentication.StopUserCancellationAsync();
            return Success();
        }
        #endregion

        #region ============ 后台 ============
        /// <summary>
        /// 前台跳转后台登录
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
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> DashboardLogin([FromBody] LoginInput input)
        {
            var cache = _cache.GetLoginOathCode(input.Email!) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, "oathcode is expired");
            if (cache.OathCode != input.OathCode)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var dto = await _authentication.LoginAsync(cache.Value!.ToMap<LoginValidateDto>(), input.Remember ?? false, true);

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
        public async Task<DataOutput<LoginViewModel>> DashboardRefreshToken([FromBody] RefreshTokenInput input)
        {
            var dto = await _authentication.RefreshTokenAsync(input.RefreshToken!, true);
            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Dashboard/Change/Password")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<LoginViewModel>> DashboardChangePassword([FromBody] ChangePasswordInput input)
        {
            var data = input.ToMap<ChangePasswordDto>();
            var dto = await _authentication.ChangePasswordAsync(data);
            return Success(dto.ToMap<LoginViewModel>());
        }

        /// <summary>
        /// 锁屏解锁
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Dashboard/Screen/UnLock")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> UnLock([FromBody] UnLockInput input)
        {
            await _authentication.ScreenUnLockAsync(input.Password!);
            return Success();
        }
        #endregion
    }
}
