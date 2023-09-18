using Lycoris.Blog.Application.AppService.Chat.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.SignalR.Chats.Dtos;
using Lycoris.Blog.Application.SignalR.Shared.Dtos;

namespace Lycoris.Blog.Application.SignalR.Chats
{
    public interface IChatHubSignalRService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<long>> GetUserChatRoomListAsync(long userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="chatUserId"></param>
        /// <returns></returns>
        Task<ChatRoomDto> CreateChatRoomAsync(SignalRConnectionDto connection, long chatUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="lastActiveTime"></param>
        /// <returns></returns>
        Task UpdateChatRoomLastActiveTimeAsync(long roomId, DateTime lastActiveTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ChatMessageDataDto> CreateChatMessageAsync(CreateChatMessageDto input);
    }
}
