namespace Lycoris.Blog.Server.Models.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class PostCommentQueryDataViewModel
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
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? OriginalContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IpAddress { get; set; }
    }
}
