using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.AccessControls
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessControlCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Ip { get; set; }
    }
}
