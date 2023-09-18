using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveAboutWebInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Value { get; set; }
    }
}
