using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.Posts;
using Lycoris.Blog.Application.AppServices.Posts.Dtos;
using Lycoris.Blog.Common;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Posts;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 博客文章
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Post")]
    public class PostController : BaseController
    {
        private readonly IPostAppService _post;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        public PostController(IPostAppService post)
        {
            _post = post;
        }

        #region ======== 博客网站 ========

        /// <summary>
        /// 推荐文章列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Recommend/List")]
        [Produces("application/json")]
        public async Task<ListOutput<PostRecommendDataViewModel>> PostRecommendList()
        {
            var dto = await _post.GetPostRecommendListAsync();
            return Success(dto.ToMapList<PostRecommendDataViewModel>());
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<PostDataViewModel>> PostList([FromQuery] PostListInput input)
        {
            var filter = input.ToMap<PostFilter>();
            var dto = await _post.GetPostListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<PostDataViewModel>());
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Detail")]
        [Produces("application/json")]
        public async Task<DataOutput<PostDetailViewModel>> PostDetail([FromQuery] long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(HttpStatusCode.NotFound, "");

            var dto = await _post.GetPostDetailAsync(id.Value);
            return Success(dto.ToMap<PostDetailViewModel>());
        }

        /// <summary>
        /// 文章 上下篇索引
        /// </summary>
        /// <returns></returns>
        [HttpGet("PreviousAndNext")]
        [Produces("application/json")]
        public async Task<DataOutput<PostPreviousAndNextViewModel>> BlogPreviousAndNext([FromQuery] long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(HttpStatusCode.NotFound, "");

            var dto = await _post.GetPostPreviousAndNextAsync(id.Value);
            return Success(dto.ToMap<PostPreviousAndNextViewModel>());
        }

        /// <summary>
        /// 文章查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("Search")]
        [Produces("application/json")]
        public async Task<ListOutput<SearchPostDataViewModel>> Search([FromQuery] string? keyword)
        {
            if (keyword.IsNullOrEmpty())
                return Success(new List<SearchPostDataViewModel>());

            var dto = await _post.SearchAsync(keyword!);

            return Success(dto.ToMapList<SearchPostDataViewModel>());
        }
        #endregion

        #region ======== 管理后台 ========

        /// <summary>
        /// 查询文章列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<PageOutput<PostQueryDataViewModel>> List([FromQuery] PostQueryListInput input)
        {
            var filter = input.ToMap<PostQueryFilter>();
            var dto = await _post.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<PostQueryDataViewModel>());
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Info")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<DataOutput<PostInfoViewModel>> Info([FromQuery] long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var dto = await _post.GetInfoAsync(id!.Value);
            return Success(dto.ToMap<PostInfoViewModel>());
        }

        /// <summary>
        /// 保存文章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Save")]
        [AppAuthentication]
        [GanssXssSettings("Markdown", "Info")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Save([FromBody] PostSaveInput input)
        {
            if (input.IsPublish ?? false)
            {
                if (input.Info.IsNullOrEmpty())
                    throw new FriendlyException("文章摘要不能为空");
                else if (input.Markdown.IsNullOrEmpty())
                    throw new FriendlyException("文章内容不能为空");
            }

            var data = input.ToMap<PostSaveDto>();
            await _post.SaveAsync(data);
            return Success();
        }

        /// <summary>
        /// Markdown文件上传接口
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("Markdown/Upload"), AppAuthentication]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<string>> MarkdownUpload([FromForm] MarkdownUploadInput input, [FromServices] IFileManageAppService fileManage)
        {
            var url = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.Post);
            return Success(url);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromBody] PostDeleteInput input)
        {
            await _post.DeleteAsync(input.Id!.Value);
            return Success();
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Publish")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<BaseOutput> Publish([FromQuery] long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            await _post.PublishPostAsync(id!.Value);
            return Success();
        }

        /// <summary>
        /// 文章评论权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Comment")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Comment([FromBody] PoseCommentInput input)
        {
            await _post.SetPostCommentAsync(input.Id!.Value, input.Comment!.Value == 1);
            return Success();
        }

        /// <summary>
        /// 设置文章推荐
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Recommend")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Recommend([FromBody] PoseRecommendInput input)
        {
            await _post.SetPostRecommendAsync(input.Id!.Value, input.Recommend!.Value == 1);
            return Success();
        }

        /// <summary>
        /// 上传文章封面图
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("Icon/Upload"), AppAuthentication]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<string>> UploadPostIcon([FromForm] UploadPostIconInput input, [FromServices] IFileManageAppService fileManage)
        {
            var url = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.PostIcon);
            return Success(url);
        }
        #endregion
    }
}
