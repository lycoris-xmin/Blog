using Lycoris.Blog.Model.Configurations;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveUploadSettingInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? UploadChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool? LocalBackup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? LoadFileSrc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MinioConfiguration? Minio { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubConfiguration? Github { get; set; }
    }
}
