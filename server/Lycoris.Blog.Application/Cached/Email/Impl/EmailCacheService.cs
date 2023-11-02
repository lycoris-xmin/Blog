using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.Email.Models;
using Lycoris.Blog.Common.Cache;

namespace Lycoris.Blog.Application.Cached.Email.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class EmailCacheService : IEmailCacheService
    {
        private readonly Lazy<IMemoryCacheManager> _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        public EmailCacheService(Lazy<IMemoryCacheManager> memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #region 邮件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <returns></returns>
        public EmailCaptchaCacheModel? GetEmailCaptcha(string email, EmailTypeEnum emailType) => _memoryCache.Value.GetMemory<EmailCaptchaCacheModel>(GetEmailCaptchaTimeKey(email, emailType));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetEmailCaptcha(string email, EmailTypeEnum emailType, EmailCaptchaCacheModel value)
        {
            email = email.ToLower();
            _memoryCache.Value.CreateMemory(GetEmailCaptchaTimeKey(email, emailType), value, value.CodeExpiredTime!.Value);
        }

        private static string GetEmailCaptchaTimeKey(string email, EmailTypeEnum emailType) => $"Captcha:Email:{emailType}:{email}";
        #endregion
    }
}
