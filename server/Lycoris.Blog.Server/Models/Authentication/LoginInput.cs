using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? OathCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Remember { get; set; }
    }
}
