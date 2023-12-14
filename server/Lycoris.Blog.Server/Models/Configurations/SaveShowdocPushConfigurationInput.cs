using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveShowdocPushConfigurationInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool? MonitoringPush { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? CPURate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? RAMRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool? CommentPush { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool? MessagePush { get; set; }
    }
}
