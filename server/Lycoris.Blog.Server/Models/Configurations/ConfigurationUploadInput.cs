using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationUploadInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? ConfigName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public IFormFile? File { get; set; }
    }
}
