namespace Lycoris.Blog.Application.Cached.EmailCache.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailCaptchaCacheDto
    {
        public EmailCaptchaCacheDto(string? Code, int ExpireTime)
        {
            this.Code = Code;
            this.CaptchaExpiredTime = DateTime.Now.AddMinutes(1);
            this.CodeExpiredTime = DateTime.Now.AddMinutes(ExpireTime);
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
