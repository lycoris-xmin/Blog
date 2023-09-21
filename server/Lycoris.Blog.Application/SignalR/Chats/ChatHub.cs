using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Utils.SensitiveWord;
using Lycoris.Blog.Application.SignalR.Chats.Dtos;
using Lycoris.Blog.Application.SignalR.Shared;
using Lycoris.Blog.Application.SignalR.Shared.Dtos;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Model.Global.Output;
using Microsoft.AspNetCore.SignalR;

namespace Lycoris.Blog.Application.SignalR.Chats
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatHub : Hub
    {
        private readonly ILycorisLogger _logger;
        private readonly ISignalRService _signalRService;
        private readonly IChatHubSignalRService _chatSignalRService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="signalRService"></param>
        /// <param name="chatSignalRService"></param>
        public ChatHub(ILycorisLoggerFactory factory, ISignalRService signalRService, IChatHubSignalRService chatSignalRService)
        {
            _logger = factory.CreateLogger<ChatHub>();
            _signalRService = signalRService;
            _chatSignalRService = chatSignalRService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await Clients.Clients(Context.ConnectionId).SendAsync("authroization");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId);
            if (connection != null)
            {
                connection.Online = false;
                connection.DisconnectedTime = DateTime.Now;
                await _signalRService.AddOrUpdateSignalRConnectionAsync(connection);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HubMethodName("userAuthroization")]
        public async Task UserAuthroizationAsync(string token)
        {
            var user = await _signalRService.AnalyzeTokenAsync(token);
            if (user == null || user.Id == 0)
            {
                await Clients.Caller.SendAsync("refreshToken");
                return;
            }

            var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId) ?? new SignalRConnectionDto() { ConnectionId = Context.ConnectionId };

            connection.UserId = user.Id;
            connection.NickName = user.NickName;
            connection.Avatar = user.Avatar;
            connection.Online = true;
            connection.ConnectedTime = DateTime.Now;

            await _signalRService.AddOrUpdateSignalRConnectionAsync(connection);

            var roomIds = await _chatSignalRService.GetUserChatRoomListAsync(user.Id);
            if (roomIds.HasValue())
            {
                foreach (var item in roomIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, item.ToString());
                }
            }

            var otherConnection = await _signalRService.CleanUserDisconnectedAsync(Context.ConnectionId, user.Id);
            if (otherConnection.HasValue())
                await Clients.Clients(otherConnection).SendAsync("logout");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("sendMessage")]
        public async Task SendMessageAsync(SendMessageInput input)
        {
            try
            {
                var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId);
                if (connection != null && connection.UserId > 0)
                {
                    var message = await _chatSignalRService.CreateChatMessageAsync(new CreateChatMessageDto()
                    {
                        RoomId = input.RoomId!.ToLong(),
                        UserId = connection.UserId,
                        NickName = connection.NickName,
                        Avatar = connection.Avatar,
                        Content = SensitiveWordMemoryStore.SensitiveWordsReplace(input.Content.Trim()),
                        CreateTime = input.CreateTime ?? DateTime.Now
                    });

                    var data = message.ToMap<ChatMessageSignalRDto>();

                    await PublishGroupMessageAsync(input.RoomId, new DataOutput<ChatMessageSignalRDto>()
                    {
                        ResCode = ResCodeEnum.Success,
                        ResMsg = "",
                        Data = data
                    });

                    data.MessageId = input.MessageId;
                    await PublishMessageAckAsync(new DataOutput<ChatMessageSignalRDto>()
                    {
                        ResCode = ResCodeEnum.Success,
                        ResMsg = "",
                        Data = data
                    });

                    // 更新聊天室最新活跃时间
                    await _chatSignalRService.UpdateChatRoomLastActiveTimeAsync(message.RoomId, message.CreateTime);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);

                // 信息发送失败需要处理的部分
                await PublishMessageAckAsync(new DataOutput<ChatMessageSignalRDto>()
                {
                    ResCode = ResCodeEnum.ChatPuhlishFailed,
                    ResMsg = "系统异常，消息发送失败",
                    Data = new ChatMessageSignalRDto(input.MessageId)
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("createChatRoom")]
        public async Task CreateChatRoomAsync(long chatUserId)
        {
            var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId);
            if (connection == null)
                return;

            var room = await _chatSignalRService.CreateChatRoomAsync(connection!, chatUserId);

            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());

            var chatConnection = await _signalRService.GetSignalRConnectionAsync(chatUserId);
            if (chatConnection != null)
                await Groups.AddToGroupAsync(chatConnection.ConnectionId, room.Id.ToString());

            //
            await Clients.Caller.SendAsync("chatRoom", room);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("UserUnreadMessageCount")]
        public async Task UpdateUserMessageCountAsync()
        {
            var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId);
            if (connection == null)
                return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task PublishMessageAsync<T>(string connectionId, T data) where T : BaseOutput => await Clients.Client(connectionId).SendAsync("message", data);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupId"></param>
        /// <param name="data"></param>
        /// <param name="otherConnectionId"></param>
        /// <returns></returns>
        private Task PublishGroupMessageAsync<T>(string groupId, T? data, params string[] otherConnectionId) where T : BaseOutput
        {
            var excepts = new List<string>() { Context.ConnectionId };
            if (otherConnectionId.HasValue())
                excepts.AddRange(otherConnectionId);

            return Clients.GroupExcept(groupId, excepts.ToArray()).SendAsync("message", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task PublishMessageAckAsync<T>(T data) where T : BaseOutput => await Clients.Caller.SendAsync("messageAck", data);
    }
}
