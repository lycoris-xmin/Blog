using Lycoris.Blog.Server.Models.Shared;

namespace Lycoris.Blog.Server.Models.Chat
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfoViewModel? User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOwner { get; set; }
    }
}
