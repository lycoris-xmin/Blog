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

        /// <summary>
        /// 
        /// </summary>
        public GitHutRespConfiguration Github { get; set; } = new GitHutRespConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public class MinioConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string MinioEndpoint { get => Endpoint?.Replace("https://", "").Replace("http://", "") ?? ""; }

        /// <summary>
        /// 
        /// </summary>
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool SSL { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string DefaultBucket { get; set; } = string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    public class GitHutRespConfiguration
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 仓库地址
        /// </summary>
        public string RepositoryUrl { get; set; } = string.Empty;

        /// <summary>
        /// 存储路径
        /// </summary>
        public string RepositoryPath { get; set; } = string.Empty;

        /// <summary>
        /// CND加速
        /// </summary>
        public string CDN { get; set; } = "https://cdn.jsdelivr.net/gh";
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
        /// Github仓库
        /// </summary>
        [Description("Github仓库")]
        Github = 10,
        /// <summary>
        /// Minio
        /// </summary>
        [Description("Minio")]
        Minio = 20,
        /// <summary>
        /// 阿里云存储
        /// </summary>
        [Description("阿里云存储")]
        OSS = 30,
        /// <summary>
        /// 腾讯云存储
        /// </summary>
        [Description("腾讯云存储")]
        COS = 40,
        /// <summary>
        /// 华为云存储
        /// </summary>
        [Description("华为云存储")]
        OBS = 50,
        /// <summary>
        /// 七牛云存储
        /// </summary>
        [Description("七牛云存储")]
        Kodo = 60
    }
}
