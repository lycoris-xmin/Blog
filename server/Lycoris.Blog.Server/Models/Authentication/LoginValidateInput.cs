using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginValidateInput
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}
