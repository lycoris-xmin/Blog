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
    [Table("StaticFile")]
    [TableIndex("UploadChannel")]
    [TableIndex("PathUrl")]
    [TableIndex("Use")]
    [TableIndex("CreateTime")]
    public class StaticFile : MySqlBaseEntity<long>
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
        [TableColumn(Required = true, DefaultValue = FileUploadChannelEnum.Local)]
        public FileUploadChannelEnum UploadChannel { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        [TableColumn(Required = false)]
        public string PathUrl { get; set; } = string.Empty;

        /// <summary>
        /// 远端访问地址
        /// </summary>
        [TableColumn(Required = false)]
        public string RemoteUrl { get; set; } = string.Empty;

        /// <summary>
        /// 文件Sha签名
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string FileSha { get; set; } = string.Empty;

        /// <summary>
        /// 本地备份
        /// </summary>
        [TableColumn(Required = true, DefaultValue = false)]
        public bool LocalBack { get; set; } = false;

        /// <summary>
        /// 是否有在使用
        /// </summary>
        [TableColumn(Required = true, DefaultValue = true)]
        public bool Use { get; set; } = true;

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
