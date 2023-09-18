namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RegisterCaptchaViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RegisterCaptchaViewModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Second"></param>
        public RegisterCaptchaViewModel(int Second)
        {
            this.Second = Second;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Second { get; set; }
    }
}
