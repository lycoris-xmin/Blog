using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// API请求记录表
    /// </summary>
    [Table("Log_Request")]
    [TableIndex("Route")]
    [TableIndex("StatusCode")]
    [TableIndex("Success")]
    [TableIndex("ElapsedMilliseconds")]
    public class RequestLog : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [TableColumn(IsPrimary = true, IsIdentity = true)]
        public override long Id { get; set; }

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
        /// 响应结果
        /// </summary>
        [TableColumn(DefaultValue = true)]
        public bool Success { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// 耗时
        /// </summary>
        public int ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [TableColumn(StringLength = 500)]
        public string Exception { get; set; } = string.Empty;

        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// 客户端IP
        /// </summary>
        public uint IP { get; set; } = 0;

        /// <summary>
        /// 客户端IP归属地
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string IPAddress { get; set; } = string.Empty;

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
