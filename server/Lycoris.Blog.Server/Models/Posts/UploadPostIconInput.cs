using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadPostIconInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public IFormFile? File { get; set; }
    }
}
