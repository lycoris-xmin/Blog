namespace Lycoris.Blog.Core.Email.DataModel
{
    public class SendEmailCaptchaDataModel
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 执行的操作
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 验证码有效期(单位：分钟)
        /// </summary>
        public int? ExpireTime { get; set; }
    }
}
