using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshTokenInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? RefreshToken { get; set; }
    }
}
