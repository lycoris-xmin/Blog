using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 来源统计
    /// </summary>
    [Table("Statistics_Referer")]
    [TableIndex("Domain", true)]
    public class RefererStatistics : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 来源
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Referer { get; set; } = string.Empty;

        /// <summary>
        /// 来源域名
        /// </summary>
        [TableColumn(StringLength = 50)]
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// 次数
        /// </summary>
        public int Count { get; set; }
    }
}
