using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PoseRecommendInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? Recommend { get; set; }
    }
}
