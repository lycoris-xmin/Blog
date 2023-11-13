using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 访问管控监控日志
    /// </summary>
    [Table("Log_AccessControl")]
    [TableIndex("Route")]
    [TableIndex("StatusCode")]
    public class AccessControlLog : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [TableColumn(IsPrimary = true, IsIdentity = true)]
        public override long Id { get; set; }

        /// <summary>
        /// 访问管控编号
        /// </summary>
        public int AccessControlId { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [TableColumn(StringLength = 10)]
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 请求路由
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Params { get; set; } = string.Empty;

        /// <summary>
        /// 响应状态码
        /// </summary>
        [TableColumn(DefaultValue = 200)]
        public uint StatusCode { get; set; } = 200;

        /// <summary>
        /// 响应内容
        /// </summary>
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 不映射字段
        /// </summary>
        [NotMapped]
        public override byte[]? RowVersion { get; set; }
    }
}
