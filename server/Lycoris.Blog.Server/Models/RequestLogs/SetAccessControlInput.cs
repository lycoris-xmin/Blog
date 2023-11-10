using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.RequestLogs
{
    /// <summary>
    /// 
    /// </summary>
    public class SetAccessControlInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Ip { get; set; }
    }
}
