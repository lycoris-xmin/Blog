using Lycoris.Blog.Server.Models.Shared;

namespace Lycoris.Blog.Server.Models.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfoViewModel? User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public int AgentFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<MessageReplyDataViewModel>? Redundancy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsSelf { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOwner { get; set; }
    }
}
