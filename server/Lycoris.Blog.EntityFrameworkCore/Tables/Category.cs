using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 文章分类
    /// </summary>
    [Table("Category")]
    [TableIndex("CreateTime")]
    public class Category : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 分类关键词
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 分类展示图
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Icon { get; set; } = "";

        /// <summary>
        /// 文章总数
        /// </summary>
        public int PostCount { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
