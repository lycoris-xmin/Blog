using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangeEmailInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, EmailRegex]
        public string? Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Captcha { get; set; }
    }
}
