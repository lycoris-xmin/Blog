using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.WebSite
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveAboutMeInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Config { get; set; }
    }
}
