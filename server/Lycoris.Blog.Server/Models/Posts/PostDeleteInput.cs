using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostDeleteInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? Id { get; set; }
    }
}
