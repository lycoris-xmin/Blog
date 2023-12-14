using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveDataClearConfigurationInput
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

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? RequestLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? BrowseLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? PostComment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? LeaveMessage { get; set; }
    }
}
