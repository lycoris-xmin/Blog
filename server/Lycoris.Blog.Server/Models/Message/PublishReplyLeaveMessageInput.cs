using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishReplyLeaveMessageInput : PublishLeaveMessageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, Range(1, int.MaxValue)]
        public int? MessageId { get; set; }
    }
}
