using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostDataDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

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
        public PostTypeEnum Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime PublishTime { get; set; }
    }
}
