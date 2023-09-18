using Lycoris.Blog.Application.AppService.LeaveMessages.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.LeaveMessages
{
    public interface ILeaveMessageAppService : IApplicationBaseService
    {
        #region ======== 博客网站 ========
        /// <summary>
        /// 获取一级留言列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResultDto<WebMessageDataDto>> GetWebMessageListAsync(int pageIndex, int pageSize);

        /// <summary>
        /// 获取二级留言列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<WebMessageReplyDataDto>> GetWebMessageReplyListAsync(WebMessageReplyListFilter input);

        /// <summary>
        /// 发布一级留言
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<WebMessageDataDto> PublishMessageAsync(string content);

        /// <summary>
        /// 发布二级留言
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<WebMessageReplyDataDto> PublishReplyMessageAsync(int messageId, string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteSelfMessageAsync(int id);
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 获取留言列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<MessageDataDto>> GetListAsync(MessageLsitFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task SetViolationMessageAsync(int id);

        /// <summary>
        /// 删除留言
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task DeleteAsync(params int[] ids);
        #endregion
    }
}
