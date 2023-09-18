using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 文章数据统计表
    /// </summary>
    [Table("Post_Statistics")]
    public class PostStatistics : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 文章编号
        /// </summary>
        [TableColumn(IsPrimary = true)]
        public override long Id { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int Browse { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int Comment { get; set; }
    }
}
