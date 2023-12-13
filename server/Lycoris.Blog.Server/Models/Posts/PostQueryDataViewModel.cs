namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostQueryDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

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
        public int? Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IsPublish { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Recommend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BrowseCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
