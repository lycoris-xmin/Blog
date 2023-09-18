using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.EmailCache.Dtos;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Cache;
using Lycoris.CSRedisCore.Extensions;

namespace Lycoris.Blog.Application.Cached.EmailCache.Impl
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
        public async Task<EmailCaptchaCacheDto?> GetEmailCaptchaAsync(string email, EmailTypeEnum emailType)
        {
            email = email.ToLower();
            if (AppSettings.Redis.Use)
                return await RedisCache.String.GetAsync<EmailCaptchaCacheDto>(GetEmailCaptchaTimeKey(email, emailType));
            else
                return _memoryCache.Value.GetMemory<EmailCaptchaCacheDto>(GetEmailCaptchaTimeKey(email, emailType));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetEmailCaptchaAsync(string email, EmailTypeEnum emailType, EmailCaptchaCacheDto value)
        {
            email = email.ToLower();
            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetEmailCaptchaTimeKey(email, emailType), value, value.CodeExpiredTime!.Value - DateTime.Now);
            else
                _memoryCache.Value.CreateMemory(GetEmailCaptchaTimeKey(email, emailType), value, value.CodeExpiredTime!.Value);
        }

        private static string GetEmailCaptchaTimeKey(string email, EmailTypeEnum emailType) => $"Captcha:Email:{emailType}:{email}";
        #endregion
    }
}
