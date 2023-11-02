using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.WebSite
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
