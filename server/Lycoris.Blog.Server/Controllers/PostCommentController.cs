using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.PostComments;
using Lycoris.Blog.Application.AppServices.PostComments.Dtos;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Comment;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 文章评论
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/PostComment")]
    public class PostCommentController : BaseApiController
    {
        private readonly IPostCommentAppService _comment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        public PostCommentController(IPostCommentAppService comment)
        {
            _comment = comment;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 文章评论列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<PostCommentDataViewModel>> CommentList([FromQuery] PostCommentListInput input)
        {
            var dto = await _comment.GetCommentListAsync(input.ToMap<PostCommentListFilter>());
            return Success(dto.Count, dto.List.ToMapList<PostCommentDataViewModel>());
        }

        /// <summary>
        /// 发布文章评论
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Publish")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<PostCommentDataViewModel>> PublishComment([FromBody] PublishCommentInput input)
        {
            var data = input.ToMap<CreatePostCommentDto>();
            var dto = await _comment.CreateCommentAsync(data);
            return Success(dto.ToMap<PostCommentDataViewModel>());
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 查询文章评论列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<PostCommentQueryDataViewModel>> List([FromQuery] PostCommentQueryListInput input)
        {
            var filter = input.ToMap<PostCommentQueryListFilter>();
            var dto = await _comment.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<PostCommentQueryDataViewModel>());
        }

        /// <summary>
        /// 删除文章评论
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [AppAuthentication]
        public async Task<BaseOutput> Delete([FromBody] PostCommentDeleteInput input)
        {
            if (input.Ids.HasValue())
                await _comment.DeleteAsync(input.Ids!.ToArray());

            return Success();
        }
        #endregion
    }
}
