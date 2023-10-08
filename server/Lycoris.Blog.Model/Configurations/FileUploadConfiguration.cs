using Newtonsoft.Json;
using System.ComponentModel;

namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class FileUploadConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public FileSaveChannelEnum SaveChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MinioConfiguration Minio { get; set; } = new MinioConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public class MinioConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string MinioEndpoint { get => Endpoint?.Replace("https://", "").Replace("http://", "") ?? ""; }

        /// <summary>
        /// 
        /// </summary>
        public string? AccessKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? SSL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? DefaultBucket { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FileSaveChannelEnum
    {
        /// <summary>
        /// 本地
        /// </summary>
        [Description("本地")]
        Local = 0,
        /// <summary>
        /// Minio
        /// </summary>
        [Description("Minio")]
        Minio = 10,
        /// <summary>
        /// 阿里云存储
        /// </summary>
        [Description("阿里云存储")]
        OSS = 20,
        /// <summary>
        /// 腾讯云存储
        /// </summary>
        [Description("腾讯云存储")]
        COS = 30,
        /// <summary>
        /// 华为云存储
        /// </summary>
        [Description("华为云存储")]
        OBS = 40,
        /// <summary>
        /// 七牛云存储
        /// </summary>
        [Description("七牛云存储")]
        Kodo = 50
    }
}
