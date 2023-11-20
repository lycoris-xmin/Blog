using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangeEmailCaptchaCodeInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, EmailRegex]
        public string? Email { get; set; }
    }
}
