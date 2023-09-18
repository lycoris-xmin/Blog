using Lycoris.Blog.EntityFrameworkCore.Constants;

namespace Lycoris.Blog.Application.AppService.Chat.Dtos
{
    public class ChatRoomDto
    {
        public long Id { get; set; }

        public long ChatUserId { get; set; }

        public string ChatUserName { get; set; } = "";

        public string ChatUserAvatar { get; set; } = "";

        public int UnreadCount { get; set; }

        public DateTime LastActiveTime { get; set; }

        public bool? IsOwner { get => ChatUserId == TableSeedData.UserData.Id ? true : null; }
    }
}
