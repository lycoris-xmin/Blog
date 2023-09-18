namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostInfoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 文章Markdown内容
        /// </summary>
        public string? Markdown { get; set; }

        /// <summary>
        /// 文章头图
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 文章分类
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// 文章评论
        /// </summary>
        public int? Comment { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPublish { get; set; }
    }
}
