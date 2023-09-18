using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class PostCommentQueryListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? UserId { get; set; }
    }
}
