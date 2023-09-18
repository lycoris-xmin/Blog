﻿using Lycoris.Blog.Application.AppService.Posts.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppService.Posts
{
    public interface IPostAppService : IApplicationBaseService
    {
        #region ======== 博客网站 ========

        /// <summary>
        /// 获取推荐文章列表
        /// </summary>
        /// <returns></returns>
        Task<List<PostRecommendDataDto>> GetPostRecommendListAsync();

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<PostDataDto>> GetPostListAsync(PostFilter input);

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PostDetailDto> GetPostDetailAsync(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PostPreviousAndNextDto> GetPostPreviousAndNextAsync(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<SearchPostDataDto>> SearchAsync(string keyword);
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<PostQueryDataDto>> GetListAsync(PostQueryFilter input);

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PostInfoDto> GetInfoAsync(long id);

        /// <summary>
        /// Markdown插件上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> MarkdownUploadAsync(IFormFile file);

        /// <summary>
        /// 保存文章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Transactional]
        Task SaveAsync(PostSaveDto input);

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transactional]
        Task DeleteAsync(long id);

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task PublishPostAsync(long id);

        /// <summary>
        /// 设置文章评论权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task SetPostCommentAsync(long id, bool comment);

        /// <summary>
        /// 设置文章推荐
        /// </summary>
        /// <param name="id"></param>
        /// <param name="recommend"></param>
        /// <returns></returns>
        Task SetPostRecommendAsync(long id, bool recommend);
        #endregion
    }
}
