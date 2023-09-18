using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Home
{

    /// <summary>
    /// 
    /// </summary>
    public class WebBrowseRecordInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? ClientOrign { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? PageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Referer { get; set; }
    }
}
