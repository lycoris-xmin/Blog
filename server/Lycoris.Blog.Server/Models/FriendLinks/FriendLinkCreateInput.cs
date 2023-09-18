using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.FriendLinks
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendLinkCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, MaxLength(30)]
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, MaxLength(255)]
        public string? Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, MaxLength(255)]
        public string? Link { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }
    }
}
