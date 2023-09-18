using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.FriendLinks
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendLinkApplyInput
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        [Required, MaxLength(30)]
        public string? Name { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [Required, MaxLength(255)]
        public string? Icon { get; set; }

        /// <summary>
        /// 网站链接
        /// </summary>
        [Required, MaxLength(255)]
        public string? Link { get; set; }

        /// <summary>
        /// 网站介绍
        /// </summary>
        public string? Description { get; set; }
    }
}
