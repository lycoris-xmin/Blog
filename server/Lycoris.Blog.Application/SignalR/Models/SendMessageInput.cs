namespace Lycoris.Blog.Application.SignalR.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SendMessageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string RoomId { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
