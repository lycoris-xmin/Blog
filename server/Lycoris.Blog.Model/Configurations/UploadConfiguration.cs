using Lycoris.Blog.Model.Exceptions;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public FileUploadChannelEnum UploadChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LocalBackup { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public LoadFileSrcEnum LoadFileSrc { get; set; } = LoadFileSrcEnum.Local;

        /// <summary>
        /// 
        /// </summary>
        public MinioConfiguration Minio { get; set; } = new MinioConfiguration();

        /// <summary>
        /// 
        /// </summary>
        public GithubConfiguration Github { get; set; } = new GithubConfiguration();
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
        public string Bucket { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public string ChangeMonioFileUrl(string? remotePath) => $"{this.Endpoint}/{this.Bucket}/{remotePath}";
    }

    /// <summary>
    /// 
    /// </summary>
    public class GithubConfiguration
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
        /// CND加速
        /// </summary>
        public string CDN { get; set; } = "https://cdn.jsdelivr.net";

        /// <summary>
        /// 
        /// </summary>
        public string CommitterName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string CommitterEmail { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public (string owner, string repo) AnalyzeRepository()
        {
            var url = this.RepositoryUrl.Replace("https://github.com/", "");

            var paths = url.Split('/');

            if (paths.Length != 2)
                throw new FriendlyException("");

            return (paths[0], paths[1].Split('.').FirstOrDefault()!);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public string ChangeJsDelivrCDNUrl(string? remotePath)
        {
            var (owner, repo) = this.AnalyzeRepository();
            return ChangeJsDelivrCDNUrl(owner, repo, remotePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public string ChangeJsDelivrCDNUrl(string owner, string repo, string? remotePath) => $"{this.CDN}/gh/{owner}/{repo}/{remotePath?.TrimStart('/') ?? ""}";
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FileUploadChannelEnum
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

    /// <summary>
    /// 
    /// </summary>
    public enum LoadFileSrcEnum
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local = 0,
        /// <summary>
        /// 远端
        /// </summary>
        Remote = 1
    }
}
