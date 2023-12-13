using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostQueryDataDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PostTypeEnum Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPublish { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Recommend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BrowseCount { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int CommentCount { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
