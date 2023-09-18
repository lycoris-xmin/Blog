using Lycoris.Blog.Application.AppService.Chat.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.Chat
{
    public interface IChatAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResultDto<ChatRoomDto>> GetChatRoomListAsync(int pageIndex, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<ChatMessageDataDto>> GetChatMessageListAsync(GetChatMessageListFilter input);
    }
}
