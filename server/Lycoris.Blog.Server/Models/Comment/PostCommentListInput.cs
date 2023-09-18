using Lycoris.Blog.Model.Global.Input;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class PostCommentListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? PostId { get; set; }
    }
}
