namespace Lycoris.Blog.Server.Models.Chat
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatRoomViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ChatUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ChatUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ChatUserAvatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastActiveTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOwner { get; set; }
    }
}
