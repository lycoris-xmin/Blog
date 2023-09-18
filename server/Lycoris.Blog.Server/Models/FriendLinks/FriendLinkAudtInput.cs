using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.FriendLinks
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendLinkAudtInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, Range(0, 2)]
        public int? Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Remark { get; set; }
    }
}
