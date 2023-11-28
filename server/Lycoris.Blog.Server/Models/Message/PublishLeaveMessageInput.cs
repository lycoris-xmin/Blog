using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishLeaveMessageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, MaxLength(100)]
        public string? Content { get; set; }
    }
}
