using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.Chat.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Application.SignalR.Models;
using Lycoris.Blog.Application.SignalR.Shared.Models;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.SignalR.Chats.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class ChatHubSignalRService : ApplicationBaseService, IChatHubSignalRService
    {
        private readonly Lazy<IRepository<ChatRoom, long>> _chatRoom;
        private readonly Lazy<IRepository<ChatRoomUser, long>> _chatRoomUser;
        private readonly Lazy<IRepository<ChatMessage, long>> _chatMessage;
        private readonly Lazy<IRepository<User, long>> _user;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatRoom"></param>
        /// <param name="chatRoomUser"></param>
        /// <param name="chatMessage"></param>
        /// <param name="user"></param>
        public ChatHubSignalRService(Lazy<IRepository<ChatRoom, long>> chatRoom,
                                     Lazy<IRepository<ChatRoomUser, long>> chatRoomUser,
                                     Lazy<IRepository<ChatMessage, long>> chatMessage,
                                     Lazy<IRepository<User, long>> user)
        {
            _chatRoom = chatRoom;
            _chatRoomUser = chatRoomUser;
            _chatMessage = chatMessage;
            _user = user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<long>> GetUserChatRoomListAsync(long userId) => _chatRoomUser.Value.GetAll().Where(x => x.UserId == userId).Select(x => x.RoomId).ToListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="chatUserId"></param>
        /// <returns></returns>
        public async Task<ChatRoomDto> CreateChatRoomAsync(SignalRConnectionModel connection, long chatUserId)
        {
            ChatRoom? room = null;

            var query = from cu in _chatRoomUser.Value.GetAll().Where(x => x.UserId == connection.UserId)
                        where _chatRoomUser.Value.GetAll().Any(x => x.RoomId == cu.RoomId && x.UserId == chatUserId)
                        select cu.RoomId;

            var roomId = await query.FirstOrDefaultAsync();

            if (roomId > 0)
                room = await _chatRoom.Value.GetAsync(roomId);

            room ??= await CreateChatRoomAsync(connection.UserId, chatUserId);

            var user = await _user.Value.GetSelectAsync(chatUserId, x => new User { Id = x.Id, NickName = x.NickName, Avatar = x.Avatar });

            return new ChatRoomDto()
            {
                Id = room.Id,
                ChatUserId = chatUserId,
                ChatUserName = user?.NickName ?? "用户已注销",
                ChatUserAvatar = user?.Avatar ?? "",
                UnreadCount = 0,
                LastActiveTime = DateTime.Now
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="lastActiveTime"></param>
        /// <returns></returns>
        public async Task UpdateChatRoomLastActiveTimeAsync(long roomId, DateTime lastActiveTime)
        {
            var room = await _chatRoom.Value.GetAsync(roomId);

            if (room != null && Math.Abs((room.LastActiveTime - lastActiveTime).TotalSeconds) > 60)
            {
                room.LastActiveTime = lastActiveTime;
                await _chatRoom.Value.UpdateFieIdsAsync(room, x => x.LastActiveTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ChatMessageDataDto> CreateChatMessageAsync(CreateChatMessageModel input)
        {
            var data = await _chatMessage.Value.CreateAsync(new ChatMessage()
            {
                RoomId = input.RoomId,
                UserId = input.UserId,
                Content = input.Content,
                CreateTime = input.CreateTime ?? DateTime.Now
            });

            return new ChatMessageDataDto()
            {
                Id = data.Id,
                RoomId = data.RoomId,
                User = new UserInfoDto()
                {
                    Id = data.UserId,
                    NickName = input.NickName,
                    Avatar = input.Avatar
                },
                Content = data.Content,
                CreateTime = data.CreateTime,
                IsOwner = data.UserId == TableSeedData.UserData.Id ? true : null
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="chatUserId"></param>
        /// <returns></returns>
        private async Task<ChatRoom> CreateChatRoomAsync(long userId, long chatUserId)
        {
            var data = await _chatRoom.Value.CreateAsync(new ChatRoom()
            {
                LastActiveTime = DateTime.Now
            });

            var roomUsers = new List<ChatRoomUser>()
            {
                    new ChatRoomUser(){  RoomId = data.Id, UserId = userId, LastActiveTime = data.LastActiveTime },
                    new ChatRoomUser(){  RoomId = data.Id, UserId = chatUserId, LastActiveTime = data.LastActiveTime }
                };

            await _chatRoomUser.Value.CreateAsync(roomUsers);

            return data;
        }
    }
}
