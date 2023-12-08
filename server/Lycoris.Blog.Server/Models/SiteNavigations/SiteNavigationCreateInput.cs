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
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? GroupName { get; set; }
    }
}
