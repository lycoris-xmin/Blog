using Lycoris.Blog.Application.AppService.FriendLinks.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.FriendLinks
{
    public interface IFriendLinkAppService : IApplicationBaseService
    {
        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<FriendLinkDataDto>> GetFriendLinkListAsync(int pageIndex, int pageSize);

        /// <summary>
        /// 友情链接申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task FriendLinkApplyAsync(FriendLinkApplyDto input);
        #endregion


        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<FriendLinkQueryDataDto>> GetListAsync(FriendLinkQueryFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<FriendLinkQueryDataDto> CreateAsync(CreateFriendLinkDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> AuditAsync(AuditFriendLinkDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);
        #endregion
    }
}
