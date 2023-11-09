using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowdocPushTestInput
    {
        /// <summary>
        /// showdoc推送地址
        /// </summary>
        [Required]
        public string? Host { get; set; }

        /// <summary>
        /// 推送标题
        /// </summary>
        [Required]
        public string? Title { get; set; }

        /// <summary>
        /// 推送内容
        /// </summary>
        [Required]
        public string? Content { get; set; }
    }
}
