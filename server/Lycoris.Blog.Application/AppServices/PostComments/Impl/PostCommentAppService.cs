﻿using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.PostComments.Dtos;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Lycoris.Common.Utils.SensitiveWord;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.PostComments.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class PostCommentAppService : ApplicationBaseService, IPostCommentAppService
    {
        private readonly IRepository<PostComment, long> _comment;
        private readonly Lazy<IRepository<User, long>> _user;
        private readonly Lazy<IRepository<Post, long>> _post;
        private readonly Lazy<IScheduleQueueCacheService> _scheduleQueue;

        public PostCommentAppService(IRepository<PostComment, long> comment,
                                 Lazy<IRepository<User, long>> user,
                                 Lazy<IRepository<Post, long>> post,
                                 Lazy<IScheduleQueueCacheService> scheduleQueue)
        {
            _comment = comment;
            _user = user;
            _post = post;
            _scheduleQueue = scheduleQueue;
        }


        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<PostCommentDataDto>> GetCommentListAsync(PostCommentListFilter input)
        {
            var filter = _comment.GetAll().Where(x => x.PostId == input.PostId);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<PostCommentDataDto>(count);

            var query = from comment in filter.OrderByDescending(x => x.CreateTime).PageBy(input.PageIndex, input.PageSize)

                        join u in _user.Value.GetAll() on comment.CreateUserId equals u.Id into uu
                        from user in uu.DefaultIfEmpty()

                        join ru in _user.Value.GetAll() on comment.RepliedUserId equals ru.Id into ruu
                        from repliedUser in ruu.DefaultIfEmpty()

                        select new PostCommentDataDto()
                        {
                            Id = comment.Id,
                            User = new UserInfoDto()
                            {
                                Id = user != null ? user.Id : 0,
                                NickName = user != null ? user.NickName : "用户已注销",
                                Avatar = user != null ? user.Avatar : "/images/404.png"
                            },
                            RepliedUserId = comment.RepliedUserId,
                            RepliedUser = comment.RepliedUserId > 0 ? repliedUser != null ? repliedUser.NickName : "用户已注销" : "",
                            Content = comment.Content,
                            AgentFlag = comment.AgentFlag,
                            IpAddress = comment.IpAddress,
                            CreateTime = comment.CreateTime
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<PostCommentDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PostCommentDataDto> CreateCommentAsync(CreatePostCommentDto input)
        {
            var post = await _post.Value.GetSelectAsync(input.PostId, x => new Post { Id = x.Id, Comment = x.Comment, IsPublish = x.IsPublish });
            if (post == null)
                throw new FriendlyException("文章或已被站长删除,无法评论");
            else if (!post.IsPublish)
                throw new FriendlyException("文章已被隐藏,无法评论");
            else if (!post.Comment)
                throw new FriendlyException("文章已被设置为禁止评论");

            var comment = new PostComment()
            {
                PostId = post.Id,
                RepliedUserId = input.RepliedUserId ?? 0,
                Content = SensitiveWordMemoryStore.SensitiveWordsReplace(input.Content.Trim()),
                UserAgent = CurrentRequest.UserAgent,
                AgentFlag = UserAgentHelper.GetUserAgent(CurrentRequest.UserAgent)?.Code ?? 0,
                Ip = IPAddressHelper.Ipv4ToUInt32(CurrentRequest.RequestIP),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(CurrentRequest.RequestIP)),
                CreateUserId = CurrentUser!.Id,
                CreateTime = DateTime.Now
            };

            if (comment.RepliedUserId == CurrentUser.Id)
                comment.RepliedUserId = 0;

            var repliedUser = "";
            if (input.RepliedUserId.HasValue && input.RepliedUserId.Value > 0)
                repliedUser = (await _user.Value.GetSelectAsync(input.RepliedUserId.Value, x => new User() { Id = x.Id, NickName = x.NickName }))?.NickName ?? "";

            comment.IpAddress = comment.IpAddress;

            comment = await _comment.CreateAsync(comment);

            _scheduleQueue.Value.Enqueue(ScheduleTypeEnum.PostStatistics, new PostStaticQueueModel() { PostId = comment.PostId, StaticType = PostStaticTypeEnum.Comment });

            return new PostCommentDataDto()
            {
                Id = comment.Id,
                User = new UserInfoDto()
                {
                    Id = CurrentUser!.Id,
                    NickName = CurrentUser!.NickName,
                    Avatar = CurrentUser.Avatar
                },
                RepliedUser = repliedUser,
                Content = comment.Content,
                AgentFlag = comment.AgentFlag,
                IpAddress = comment.IpAddress,
                CreateTime = comment.CreateTime
            };
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<PostCommentQueryDataDto>> GetListAsync(PostCommentQueryListFilter input)
        {
            var filter = _comment.GetAll()
                                 .WhereIf(input.UserId.HasValue, x => x.CreateUserId == input.UserId)
                                 .WhereIf(!input.Content.IsNullOrEmpty(), x => EF.Functions.Like(x.Content, $"%{input.Content}%"));

            var query = from comment in filter.OrderByDescending(x => x.CreateTime)

                        join p in _post.Value.GetAll() on comment.PostId equals p.Id into pp
                        from post in pp.DefaultIfEmpty()

                        join u in _user.Value.GetAll() on comment.CreateUserId equals u.Id into uu
                        from user in uu.DefaultIfEmpty()

                        select new PostCommentQueryDataDto()
                        {
                            Id = comment.Id,
                            Title = post != null ? post.Title : "文章不存在",
                            Content = comment.Content,
                            IsOwner = user != null && user.IsAdmin,
                            UserName = user != null ? user.NickName : "用户已注销",
                            IpAddress = comment.IpAddress,
                            CreateTime = comment.CreateTime
                        };

            query = query.WhereIf(!input.Title.IsNullOrEmpty(), x => EF.Functions.Like(x.Title!, $"%{input.Title}%"));

            var count = await query.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<PostCommentQueryDataDto>(count);

            var list = await query.PageBy(input.PageIndex, input.PageSize).ToListAsync();

            return new PageResultDto<PostCommentQueryDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(params long[] id)
        {
            if (!id.HasValue())
                return;

            await _comment.ExecuteNonQueryAsync($"DELETE FROM {_comment.TableName} WHERE Id IN ({string.Join(",", id)})");
        }
        #endregion
    }
}
