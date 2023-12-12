using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostSaveInput
    {
        /// <summary>
        /// 
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringValid("标题", Required = true, MinLength = 1, MaxLength = 80)]
        public string? Title { get; set; }

        /// <summary>
        /// 文章摘要
        /// </summary>
        [StringValid("文章摘要", MinLength = 1, MaxLength = 200)]
        public string? Info { get; set; }

        /// <summary>
        /// 文章Markdown内容
        /// </summary>
        [StringValid("文章内容", MinLength = 1)]
        public string? Markdown { get; set; }

        /// <summary>
        /// 文章头图
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [Required]
        public PostTypeEnum? Type { get; set; }

        /// <summary>
        /// 文章分类
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// 文章评论
        /// </summary>
        [Required, Range(0, 1)]
        public int? Comment { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Recommend { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [Required]
        public bool? IsPublish { get; set; }
    }
}
