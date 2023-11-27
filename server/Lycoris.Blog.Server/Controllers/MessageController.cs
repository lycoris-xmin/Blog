using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Message;
using Lycoris.Blog.Application.AppServices.Message.Dtos;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.LeaveMessages;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 网站留言
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Message")]
    public class MessageController : BaseController
    {
        private readonly IMessageAppService _message;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MessageController(IMessageAppService message)
        {
            _message = message;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 网站留言列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<MessageDataViewModel>> LeaveMessageList([FromQuery] PageInput input)
        {
            var dto = await _message.GetWebMessageListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<MessageDataViewModel>());
        }

        /// <summary>
        /// 二级留言列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Reply/List")]
        [Produces("application/json")]
        public async Task<ListOutput<MessageReplyDataViewModel>> LeaveMessageReplyList([FromQuery] MessageReplyListInput input)
        {
            var dto = await _message.GetWebMessageReplyListAsync(input.ToMap<WebMessageReplyListFilter>());
            return Success(dto.ToMapList<MessageReplyDataViewModel>());
        }

        /// <summary>
        /// 发布留言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Publish")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<MessageDataViewModel>> PublishLeaveMessage([FromBody] PublishLeaveMessageInput input)
        {
            var dto = await _message.PublishMessageAsync(input.Content!);
            return Success(dto.ToMap<MessageDataViewModel>());
        }

        /// <summary>
        /// 发布二级留言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Reply/Publish")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<MessageReplyDataViewModel>> PublishReplyMessage([FromBody] PublishReplyLeaveMessageInput input)
        {
            var dto = await _message.PublishReplyMessageAsync(input.MessageId!.Value, input.Content!);
            return Success(dto.ToMap<MessageReplyDataViewModel>());
        }

        /// <summary>
        /// 删除自己的留言
        /// </summary>
        /// <returns></returns>
        [HttpPost("Self/Delete")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> DeleteSelfMessage([FromBody] SingleIdInput<int?> input)
        {
            await _message.DeleteSelfMessageAsync(input.Id!.Value);
            return Success();
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 查询网站留言列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<MessageQueryDataViewModel>> List([FromQuery] MessageQueryListInput input)
        {
            var filter = input.ToMap<MessageLsitFilter>();
            var dto = await _message.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<MessageQueryDataViewModel>());
        }

        /// <summary>
        /// 设置为违规留言
        /// </summary>
        /// <returns></returns>
        [HttpPost("Violation"), AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SetViolationMessage([FromBody] SingleIdInput<int?> input)
        {
            await _message.SetViolationMessageAsync(input.Id!.Value);
            return Success();
        }

        /// <summary>
        /// 删除留言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete"), AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromBody] SingleIdArrayInput<int> input)
        {
            await _message.DeleteAsync(input.Ids!);
            return Success();
        }
        #endregion
    }
}
