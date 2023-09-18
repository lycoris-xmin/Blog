using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 文章表
    /// </summary>
    [Table("Post")]
    [TableIndex("Category")]
    [TableIndex("Type")]
    [TableIndex("UpdateTime")]
    [TableIndex(new[] { "IsPublish", "Recommend" })]
    public class Post : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 标题
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 简介
        /// </summary>
        [TableColumn(StringLength = 600)]
        public string Info { get; set; } = string.Empty;

        /// <summary>
        /// 文章Markdown内容
        /// </summary>
        public string Markdown { get; set; } = string.Empty;

        /// <summary>
        /// 文章头图
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// 文章类型：0-原创，1-转载
        /// </summary>
        [TableColumn(ColumnType = MySqlType.TINYINT, DefaultValue = PostTypeEnum.Original)]
        public PostTypeEnum Type { get; set; }

        /// <summary>
        /// 文章分类
        /// </summary>
        [TableColumn(DefaultValue = 0)]
        public int Category { get; set; } = 0;

        /// <summary>
        /// 文章评论: 0-禁止留言，1-允许留言
        /// </summary>
        [TableColumn(DefaultValue = true)]
        public bool Comment { get; set; } = true;

        /// <summary>
        /// 推荐: 0-未设置，1-推荐
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool Recommend { get; set; } = false;

        /// <summary>
        /// 文章标签
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Tags { get; set; } = "";

        /// <summary>
        /// 是否发布: 0-否，1-是
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool IsPublish { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

