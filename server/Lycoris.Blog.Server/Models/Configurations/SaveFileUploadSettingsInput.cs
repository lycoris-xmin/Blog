using Lycoris.Blog.Model.Configurations;
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
        public MinioConfiguration? Minio { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubConfiguration? Github { get; set; }
    }
}
