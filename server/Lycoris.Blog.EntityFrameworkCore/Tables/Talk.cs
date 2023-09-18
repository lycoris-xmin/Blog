using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 说说
    /// </summary>
    [Table("Talk")]
    public class Talk : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 吐槽内容
        /// </summary>
        [TableColumn(StringLength = 1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 客户端标识
        /// </summary>
        [TableColumn(ColumnType = MySqlType.TINYINT)]
        public int AgentFlag { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string UserAgent { get; set; } = "";

        /// <summary>
        /// ip地址
        /// </summary>
        [Comment("ip地址")]
        public uint Ip { get; set; }

        /// <summary>
        /// IP归属地
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// 发布时间
        /// </summary>
        [Comment("发布时间")]
        public DateTime CreateTime { get; set; }
    }
}
