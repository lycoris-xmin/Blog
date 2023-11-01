namespace Lycoris.Blog.Cache.Email.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailCaptchaCacheModel
    {
        public EmailCaptchaCacheModel(string? Code, int ExpireTime)
        {
            this.Code = Code;
            CaptchaExpiredTime = DateTime.Now.AddMinutes(1);
            CodeExpiredTime = DateTime.Now.AddMinutes(ExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CaptchaExpiredTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CodeExpiredTime { get; set; }
    }
}
