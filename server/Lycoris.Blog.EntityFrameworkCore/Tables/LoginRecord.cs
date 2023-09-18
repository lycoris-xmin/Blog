using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 用户登录记录表
    /// </summary>
    [Table("Login_Record")]
    [TableIndex(new[] { "UserId" })]
    [TableIndex(new[] { "Ip", "UserAgent" })]
    [TableIndex(new[] { "LoginTime" })]
    public class LoginRecord : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 登录客户端
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string UserAgent { get; set; } = "";

        /// <summary>
        /// ip地址
        /// </summary>
        [TableColumn(DefaultValue = 0)]
        public uint Ip { get; set; }

        /// <summary>
        /// IP归属地
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登录备注
        /// </summary>
        [TableColumn(StringLength = 255, Required = false)]
        public string Remark { get; set; } = "";

        /// <summary>
        /// 不映射字段
        /// </summary>
        [NotMapped]
        public override byte[]? RowVersion { get; set; }
    }
}
