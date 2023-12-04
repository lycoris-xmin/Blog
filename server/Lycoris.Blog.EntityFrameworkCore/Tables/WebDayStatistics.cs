using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站日数据统计表
    /// </summary>
    [Table("Statistics_Web_Day")]
    public class WebDayStatistics : MySqlBaseEntity<DateTime>
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        [Key]
        [TableColumn(ColumnType = MySqlType.DATE)]
        public override DateTime Id { get; set; }

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

        /// <summary>
        /// 新增用户数
        /// </summary>
        public int User { get; set; }

        /// <summary>
        /// 新增留言数
        /// </summary>
        public int CommentMessage { get; set; }
    }
}
