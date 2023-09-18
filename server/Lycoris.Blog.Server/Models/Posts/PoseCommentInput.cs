using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PoseCommentInput
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
        public int? Comment { get; set; }
    }
}
