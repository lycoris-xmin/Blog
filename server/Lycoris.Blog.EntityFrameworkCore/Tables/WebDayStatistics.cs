using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站日数据统计表
    /// </summary>
    [Table("Web_Day_Statistics")]
    [TableIndex("Day", true)]
    public class WebDayStatistics : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        [TableColumn(ColumnType = MySqlType.DATE)]
        public DateTime Day { get; set; }

        /// <summary>
        /// 浏览量(PV)
        /// </summary>
        public int PVBrowse { get; set; }

        /// <summary>
        /// 访客数(UV)
        /// </summary>
        public int UVBrowse { get; set; }

        /// <summary>
        /// 接口调用量
        /// </summary>
        public int Api { get; set; }

        /// <summary>
        /// 异常请求Api数量
        /// </summary>
        public int ErrorApi { get; set; }
    }
}
