using Lycoris.Blog.Cache.Email.Models;

namespace Lycoris.Blog.Cache.Email
{
    public interface IEmailCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <returns></returns>
        EmailCaptchaCacheModel? GetEmailCaptcha(string email, EmailTypeEnum emailType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void SetEmailCaptcha(string email, EmailTypeEnum emailType, EmailCaptchaCacheModel value);
    }
}
