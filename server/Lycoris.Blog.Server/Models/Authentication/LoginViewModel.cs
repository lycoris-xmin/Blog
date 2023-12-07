namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime RefreshTokenExpireTime { get; set; }
    }
}
