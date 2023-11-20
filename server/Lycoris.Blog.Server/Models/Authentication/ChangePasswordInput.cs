using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangePasswordInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, PasswordRegex]
        public string? OldPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, PasswordRegex]
        public string? Password { get; set; }
    }
}
