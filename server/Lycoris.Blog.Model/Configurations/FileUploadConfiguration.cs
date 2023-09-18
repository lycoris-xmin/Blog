using Newtonsoft.Json;

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
        public int SaveChannel { get; set; }

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
}
