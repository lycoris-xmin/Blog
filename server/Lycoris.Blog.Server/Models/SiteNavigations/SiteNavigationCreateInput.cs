using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.SiteNavigations
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNavigationCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Group { get; set; }
    }
}
