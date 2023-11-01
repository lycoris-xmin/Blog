using Lycoris.Blog.Application.AppServices.Comment.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Comment
{
    public interface ICommentAppService : IApplicationBaseService
    {
        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<PostCommentDataDto>> GetCommentListAsync(PostCommentListFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PostCommentDataDto> CreateCommentAsync(CreatePostCommentDto input);
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<PostCommentQueryDataDto>> GetListAsync(PostCommentQueryListFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(params long[] id);
        #endregion
    }
}
