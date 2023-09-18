using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 来源域名统计表
    /// </summary>
    [Table("Browse_Referer")]
    [TableIndex(new[] { "Referer" })]
    public class BrowseReferer : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 来源域名
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// 来源地址
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Referer { get; set; } = string.Empty;

        /// <summary>
        /// 统计次数
        /// </summary>
        public int Count { get; set; } = 0;
    }
}
