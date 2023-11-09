using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveSystemFileClearConfigurationInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? StaticFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? TempFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? LogFile { get; set; }
    }
}
