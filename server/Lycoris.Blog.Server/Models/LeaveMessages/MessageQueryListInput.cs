using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.LeaveMessages
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageQueryListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Status { get; set; }
    }
}
