using Lycoris.Blog.Core.Email.DataModel;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;

namespace Lycoris.Blog.Core.Email
{
    public interface IEmailAppService
    {
        /// <summary>
        /// 测试邮件(主要用来管理员测试邮箱配置是否正确)
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        Task SendTestEmailAsync(string emailAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="option"></param>
        /// <param name="basic"></param>
        /// <returns></returns>
        Task SendTestEmailAsync(string emailAddress, EmailSettingsConfiguration option, WebSettingsConfiguration? basic = null);

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SendEmailCaptchaAsync(SendEmailCaptchaDataModel input);

        /// <summary>
        /// 重置密码邮件
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="emailValidId"></param>
        /// <returns></returns>
        Task SendResetPasswordEmailAsync(string nickName, string emailAddress, long emailValidId);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SendAsync(SendEmailDataModel input);
    }
}
