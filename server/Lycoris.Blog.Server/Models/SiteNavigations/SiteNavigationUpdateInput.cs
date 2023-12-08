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
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? GroupName { get; set; }
    }
}
