using Lycoris.Blog.Application.AppServices.Chat.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.SignalR.Models;
using Lycoris.Blog.Application.SignalR.Shared.Models;

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
        Task<ChatRoomDto> CreateChatRoomAsync(SignalRConnectionModel connection, long chatUserId);

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
        Task<ChatMessageDataDto> CreateChatMessageAsync(CreateChatMessageModel input);
    }
}
