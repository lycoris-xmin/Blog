namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PublishTime { get; set; }
    }
}
