namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailCaptchaViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public EmailCaptchaViewModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Second"></param>
        public EmailCaptchaViewModel(int Second)
        {
            this.Second = Second;
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Second { get; set; }
    }
}
