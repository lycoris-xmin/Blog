namespace Lycoris.Blog.Application.SignalR.Models
{
    public class CreateChatMessageModel
    {
        public long RoomId { get; set; }

        public long UserId { get; set; }

        public string NickName { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime? CreateTime { get; set; }
    }
}
