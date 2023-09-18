using Lycoris.Blog.Application.Cached.EmailCache.Dtos;

namespace Lycoris.Blog.Application.Cached.EmailCache
{
    public interface IEmailCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <returns></returns>
        Task<EmailCaptchaCacheDto?> GetEmailCaptchaAsync(string email, EmailTypeEnum emailType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetEmailCaptchaAsync(string email, EmailTypeEnum emailType, EmailCaptchaCacheDto value);
    }
}
