using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Chat.Dtos
{
    public class GetChatMessageListFilter : PageFilter
    {
        public long RoomId { get; set; }
    }
}
