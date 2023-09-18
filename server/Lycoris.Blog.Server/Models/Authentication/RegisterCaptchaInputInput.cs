using Lycoris.Blog.Server.PropertyAttribute;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RegisterCaptchaInputInput
    {
        /// <summary>
        /// 
        /// </summary>
        [EmailValid(Required = true)]
        public string? Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, Range(1, 2)]
        public int? ActionType { get; set; }
    }
}
