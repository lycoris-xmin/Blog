using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveShowdocPushConfigurationInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Host { get; set; }
    }
}
