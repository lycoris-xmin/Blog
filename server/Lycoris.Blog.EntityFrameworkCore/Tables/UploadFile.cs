using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.Model.Configurations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 上传文件
    /// </summary>
    [Table("UploadFile")]
    [TableIndex(new[] { "SaveChannel", "Use" })]
    public class UploadFile : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 文件访问路径
        /// </summary>
        [TableColumn(StringLength = 255, Required = true)]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 文件名称
        /// </summary>
        [TableColumn(StringLength = 255, Required = true)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 存储位置
        /// </summary>
        [TableColumn(Required = true, DefaultValue = FileSaveChannelEnum.Local)]
        public FileSaveChannelEnum SaveChannel { get; set; }

        /// <summary>
        /// Github文件签名
        /// </summary>
        public string GithubSha { get; set; } = string.Empty;

        /// <summary>
        /// 是否有在使用
        /// </summary>
        [TableColumn(Required = true, DefaultValue = false)]
        public bool Use { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string FilePath => System.IO.Path.Combine(AppSettings.Path.WebRootPath, this.Path.TrimStart('/'), this.FileName);
    }
}
