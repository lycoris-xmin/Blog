using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.SignalR.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageSignalRModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ChatMessageSignalRModel() { }

        /// <summary>
        /// 
        /// </summary>
        public ChatMessageSignalRModel(string MessageId)
        {
            this.MessageId = MessageId;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string? RoomId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfoDto? User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOwner { get; set; }
    }
}
