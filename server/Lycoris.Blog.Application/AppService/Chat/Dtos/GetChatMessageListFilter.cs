using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.Chat.Dtos
{
    public class GetChatMessageListFilter : PageFilter
    {
        public long RoomId { get; set; }
    }
}
