using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.Chat.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.Chat.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class ChatAppService : ApplicationBaseService, IChatAppService
    {
        private readonly IRepository<ChatRoom, long> _chatRoom;
        private readonly IRepository<ChatRoomUser, long> _chatRoomUser;
        private readonly IRepository<ChatMessage, long> _chatMessage;
        private readonly Lazy<IRepository<User, long>> _user;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatRoom"></param>
        /// <param name="chatRoomUser"></param>
        /// <param name="chatMessage"></param>
        /// <param name="user"></param>
        public ChatAppService(IRepository<ChatRoom, long> chatRoom,
                              IRepository<ChatRoomUser, long> chatRoomUser,
                              IRepository<ChatMessage, long> chatMessage,
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
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResultDto<ChatRoomDto>> GetChatRoomListAsync(int pageIndex, int pageSize)
        {
            var filter = _chatRoomUser.GetAll().Where(x => x.UserId == CurrentUser!.Id!);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<ChatRoomDto>(count);

            // 先拿私信聊天室编号
            var roomQuery = from ru in filter.OrderByDescending(x => x.LastActiveTime)

                            join r in _chatRoom.GetAll() on ru.RoomId equals r.Id into rr
                            from room in rr.DefaultIfEmpty()

                            select room != null ? room.Id : 0;

            var roomIds = await roomQuery.Where(x => x > 0).PageBy(pageIndex, pageSize).ToListAsync();

            // 再根据聊天室编号获取完整数据
            var query = from room in _chatRoom.GetAll().Where(x => roomIds.Contains(x.Id))

                        join c in _chatRoomUser.GetAll().Where(x => x.UserId != CurrentUser!.Id!) on room.Id equals c.RoomId into cc
                        from roomUser in cc.DefaultIfEmpty()

                        join u in _user.Value.GetAll() on roomUser.UserId equals u.Id into uu
                        from user in uu.DefaultIfEmpty()

                        select new ChatRoomDto()
                        {
                            Id = room != null ? room.Id : 0,
                            ChatUserId = user != null ? user.Id : 0,
                            ChatUserName = user != null ? user.NickName : "用户已注销",
                            ChatUserAvatar = user != null ? user.Avatar : "",
                            UnreadCount = 0,
                            LastActiveTime = room.LastActiveTime
                        };


            var list = await query.ToListAsync();

            return new PageResultDto<ChatRoomDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<ChatMessageDataDto>> GetChatMessageListAsync(GetChatMessageListFilter input)
        {
            var filter = _chatMessage.GetAll().Where(x => x.RoomId == input.RoomId);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<ChatMessageDataDto>(count);

            var query = from message in filter.OrderByDescending(x => x.CreateTime).PageBy(input.PageIndex, input.PageSize)

                        join u in _user.Value.GetAll() on message.UserId equals u.Id into uu
                        from user in uu.DefaultIfEmpty()

                        select new ChatMessageDataDto()
                        {
                            Id = message.Id,
                            Content = message.Content,
                            User = new UserInfoDto()
                            {
                                Id = user != null ? user.Id : 0,
                                NickName = user != null ? user.NickName : "用户已注销",
                                Avatar = user != null ? user.Avatar : "",
                            },
                            CreateTime = message.CreateTime,
                            IsOwner = user != null && user.IsAdmin
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<ChatMessageDataDto>(count, list.OrderBy(x => x.CreateTime).ToList());
        }
    }
}
