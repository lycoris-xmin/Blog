using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 服务监控
    /// </summary>
    [Table("ServerMonitor")]
    [TableIndex("Time", true)]
    public class ServerMonitor : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 监控时间
        /// </summary>
        [TableColumn(ColumnType = MySqlType.DATE)]
        public DateTime Time { get; set; }

        /// <summary>
        /// CPU峰值占用率
        /// </summary>
        public int MaxCPURate { get; set; } = 0;

        /// <summary>
        /// 内存峰值占用率
        /// </summary>
        public int MaxRAMRate { get; set; } = 0;
    }
}
