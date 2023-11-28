using Lycoris.Blog.Server.Models.Shared;

namespace Lycoris.Blog.Server.Models.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageReplyDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfoViewModel User { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public LeaveMessageRepliedUserViewModel? RepliedUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

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

    /// <summary>
    /// 
    /// </summary>
    public class LeaveMessageRepliedUserViewModel : UserInfoViewModel
    {
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
