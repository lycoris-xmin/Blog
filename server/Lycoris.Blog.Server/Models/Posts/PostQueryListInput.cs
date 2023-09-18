using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostQueryListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? IsPublish { get; set; }
    }
}
