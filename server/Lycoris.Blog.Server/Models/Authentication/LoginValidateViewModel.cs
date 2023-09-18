namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginValidateViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LoginValidateViewModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OathCode"></param>
        /// <param name="GoogleAuthentication"></param>
        public LoginValidateViewModel(string? OathCode, bool? GoogleAuthentication)
        {
            this.OathCode = OathCode;
            this.GoogleAuthentication = GoogleAuthentication;
        }

        /// <summary>
        /// 授权码
        /// </summary>
        public string? OathCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? GoogleAuthentication { get; set; }
    }
}
