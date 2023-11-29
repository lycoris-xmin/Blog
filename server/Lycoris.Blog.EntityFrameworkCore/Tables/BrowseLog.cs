using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Common.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站浏览日志表
    /// </summary>
    [Table("Log_Browse")]
    [TableIndex("Route")]
    [TableIndex("Ip")]
    [TableIndex("Referer")]
    [TableIndex(new[] { "CreateTime", "ClientOrign" })]
    public class BrowseLog : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [TableColumn(IsPrimary = true, IsIdentity = true)]
        public override long Id { get; set; }

        /// <summary>
        /// 客户端记录标识
        /// </summary>
        [TableColumn(StringLength = 50)]
        public string ClientOrign { get; set; } = "";

        /// <summary>
        /// 访问页面地址
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Route { get; set; } = "";

        /// <summary>
        /// 访问页面名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string PageName { get; set; } = "";

        /// <summary>
        /// 客户端标识
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string UserAgent { get; set; } = "";

        /// <summary>
        /// 来源地址
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string? Referer { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public uint Ip { get; set; }

        /// <summary>
        /// Ip归属地
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// 国家
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Country { get; set; } = "";

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 不映射字段
        /// </summary>
        [NotMapped]
        public override byte[]? RowVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateOrignBrowse() => GenerateOrignBrowse(this.Route, this.Referer, this.Ip);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GenerateOrignBrowse(string route, string? referer, uint ip) => SecretHelper.SHA1Encrypt($"{route}{referer ?? ""}{ip}");
    }
}
