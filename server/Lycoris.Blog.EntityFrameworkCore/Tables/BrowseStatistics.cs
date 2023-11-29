using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 访问统计
    /// </summary>
    [Table("Statistics_Browse")]
    [TableIndex("Route", true)]
    public class BrowseStatistics : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 访问路由
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// 访问页面
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string PageName { get; set; } = string.Empty;

        /// <summary>
        /// 次数
        /// </summary>
        public int Count { get; set; }
    }
}
