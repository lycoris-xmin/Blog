using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 访问管控
    /// </summary>
    [Table("Access_Control")]
    [TableIndex("Ip", true)]
    public class AccessControl : MySqlBaseEntity<int>
    {
        /// <summary>
        /// Ip地址
        /// </summary>
        public uint Ip { get; set; }

        /// <summary>
        /// ip归属地
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 异常访问次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 最后一次访问时间
        /// </summary>
        public DateTime LastAccessTime { get; set; }
    }
}
