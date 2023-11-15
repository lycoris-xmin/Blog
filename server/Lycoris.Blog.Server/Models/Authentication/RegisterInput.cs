using Lycoris.Blog.Server.PropertyAttribute;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RegisterInput
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

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}
