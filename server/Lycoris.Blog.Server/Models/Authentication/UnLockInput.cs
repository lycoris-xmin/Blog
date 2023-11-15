using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class UnLockInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, PasswordRegex]
        public string? Password { get; set; }
    }
}
