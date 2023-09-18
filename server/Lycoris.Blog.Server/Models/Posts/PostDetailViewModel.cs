namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostDetailViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Markdown { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PublishTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Browse { get; set; }
    }
}
