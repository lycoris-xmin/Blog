using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.SiteNavigations
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNavigationUpdateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Group { get; set; }
    }
}
