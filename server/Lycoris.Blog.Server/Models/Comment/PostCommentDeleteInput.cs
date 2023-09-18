using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class PostCommentDeleteInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public List<long>? Ids { get; set; }
    }
}
