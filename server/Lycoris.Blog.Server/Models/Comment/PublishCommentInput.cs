using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishCommentInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? PostId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? CommentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringValid("内容", Required = true, MinLength = 1, MaxLength = 100)]
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? RepliedUserId { get; set; }
    }
}
