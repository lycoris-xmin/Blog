using Lycoris.Blog.Application.Cached.Email.Models;

namespace Lycoris.Blog.Application.Cached.Email
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
