namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostPreviousAndNextViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public BlogPreviousAndNextDataViewModel? Previous { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BlogPreviousAndNextDataViewModel? Next { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BlogPreviousAndNextDataViewModel
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
        public string? Icon { get; set; }
    }
}
