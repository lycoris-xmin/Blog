using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveAboutMeInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Config { get; set; }
    }
}
