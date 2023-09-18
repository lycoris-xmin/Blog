using Lycoris.Blog.Model.Global.Input;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.LeaveMessages
{
    /// <summary>
    /// 
    /// </summary>
    public class LeaveMessageReplyListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? MessageId { get; set; }
    }
}
