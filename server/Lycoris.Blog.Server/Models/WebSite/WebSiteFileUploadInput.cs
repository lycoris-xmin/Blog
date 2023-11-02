using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.WebSite
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSiteFileUploadInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public IFormFile? File { get; set; }
    }
}
