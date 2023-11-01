using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Chat.Dtos
{
    public class ChatMessageDataDto
    {
        public long Id { get; set; }

        public long RoomId { get; set; }

        public string? Content { get; set; }

        public UserInfoDto? User { get; set; }

        public DateTime CreateTime { get; set; }

        public bool? IsOwner { get; set; }
    }
}
