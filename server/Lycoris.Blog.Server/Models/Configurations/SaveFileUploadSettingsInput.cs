using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveFileUploadSettingsInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? SaveChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SaveMinioSettings? Minio { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SaveMinioSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Endpoint { get; set; }

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
