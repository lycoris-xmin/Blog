using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Chat;
using Lycoris.Blog.Application.AppServices.Chat.Dtos;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Chat;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Chat"), WebAuthentication(IsRequired = true)]
    public class ChatController : BaseController
    {
        private readonly IChatAppService _chat;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chat"></param>
        public ChatController(IChatAppService chat)
        {
            _chat = chat;
        }

        /// <summary>
        /// 聊天窗口列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Room/List")]
        [Produces("application/json")]
        public async Task<PageOutput<ChatRoomViewModel>> ChatRoomList([FromQuery] PageInput input)
        {
            var dto = await _chat.GetChatRoomListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<ChatRoomViewModel>());
        }

        /// <summary>
        /// 聊天记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Message/List")]
        [Produces("application/json")]
        public async Task<PageOutput<ChatMessageDataViewModel>> ChatMessageList([FromQuery] ChatMessageListInput input)
        {
            var filter = input.ToMap<GetChatMessageListFilter>();
            var dto = await _chat.GetChatMessageListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<ChatMessageDataViewModel>());
        }
    }
}
