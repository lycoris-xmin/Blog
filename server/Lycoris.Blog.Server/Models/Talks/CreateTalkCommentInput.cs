using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Talks
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateTalkCommentInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? TalkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, Range(1, 100)]
        public string? Content { get; set; }
    }
}
