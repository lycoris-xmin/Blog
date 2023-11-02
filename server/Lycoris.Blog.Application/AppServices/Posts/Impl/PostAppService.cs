using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.Posts.Dtos;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Lycoris.Blog.Application.AppServices.Posts.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class PostAppService : ApplicationBaseService, IPostAppService
    {
        private readonly IRepository<Post, long> _post;
        private readonly IRepository<Category, int> _category;
        private readonly Lazy<IRepository<PostStatistics, long>> _statistics;
        private readonly Lazy<IFileManageAppService> _fileManage;
        private readonly Lazy<IScheduleQueueCacheService> _scheduleQueue;

        public PostAppService(IRepository<Post, long> post,
                              IRepository<Category, int> category,
                              Lazy<IRepository<PostStatistics, long>> statistics,
                              Lazy<IFileManageAppService> fileManage,
                              Lazy<IScheduleQueueCacheService> scheduleQueue)
        {
            _post = post;
            _category = category;
            _statistics = statistics;
            _fileManage = fileManage;
            _scheduleQueue = scheduleQueue;
        }

        #region ======== 博客网站 ========

        /// <summary>
        /// 获取推荐文章列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<PostRecommendDataDto>> GetPostRecommendListAsync()
        {
            var filter = _post.GetAll().Where(x => x.IsPublish == true);

            var query = filter.Where(x => x.Recommend == true).PageBy(1, 6).Select(x => new PostRecommendDataDto()
            {
                Id = x.Id,
                Title = x.Title,
                Info = x.Info,
                Recommend = true
            });

            var list = await query.ToListAsync();

            if (list.HasValue())
                return list;

            query = filter.OrderByDescending(x => x.CreateTime).PageBy(1, 3).Select(x => new PostRecommendDataDto()
            {
                Id = x.Id,
                Title = x.Title,
                Info = x.Info,
            });

            return await query.ToListAsync();
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<PostDataDto>> GetPostListAsync(PostFilter input)
        {
            var filter = _post.GetAll().Where(x => x.IsPublish == true)
                                       .WhereIf(!input.Category.HasValue, x => x.Recommend == false)
                                       .WhereIf(input.Category.HasValue, x => x.Category == input.Category!.Value)
                                       .WhereIf(!input.Title.IsNullOrEmpty(), x => EF.Functions.Like(x.Title, $"%{input.Title}%"));

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<PostDataDto>(count);

            filter = filter.OrderByDescending(x => x.CreateTime);

            var query = from post in filter.PageBy(input.PageIndex, input.PageSize)

                        join c in _category.GetAll() on post.Category equals c.Id into cc
                        from _category in cc.DefaultIfEmpty()

                        select new PostDataDto()
                        {
                            Id = post.Id,
                            Type = post.Type,
                            Icon = post.Icon,
                            Title = post.Title,
                            Info = post.Info,
                            CategoryName = _category != null ? _category.Name : "",
                            Tags = post.Tags,
                            PublishTime = post.UpdateTime
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<PostDataDto>(count, list);
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PostDetailDto> GetPostDetailAsync(long id)
        {
            var data = await _post.GetAsync(id);
            if (data == null)
                throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "");
            else if (!data.IsPublish)
                throw new OutputException(ResCodeEnum.ArticleNotPublish, "文章正在被编辑，暂时无法查看");

            var res = data.ToMap<PostDetailDto>();

            if (res.Category > 0)
                res.CategoryName = await _category.GetSelectAsync(res.Category, x => x.Name) ?? "";

            res.Browse = new Random().Next(0, 1000);

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PostPreviousAndNextDto> GetPostPreviousAndNextAsync(long id)
        {
            var preFilter = _post.GetAll().Where(x => x.Id < id).Where(x => x.IsPublish == true).OrderByDescending(x => x.Id).Take(1);
            var nextFilter = _post.GetAll().Where(x => x.Id > id).Where(x => x.IsPublish == true).OrderBy(x => x.Id).Take(1);

            var query = preFilter.Union(nextFilter).Select(x => new BlogPreviousAndNextDataDto()
            {
                Id = x.Id,
                Title = x.Title,
                Icon = x.Icon
            });

            var list = await query.ToListAsync();

            return new PostPreviousAndNextDto(list.SingleOrDefault(x => x.Id < id), list.SingleOrDefault(x => x.Id > id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<List<SearchPostDataDto>> SearchAsync(string keyword)
        {
            var filter = _post.GetAll()
                              .Where(x => x.IsPublish == true)
                              .Where(x => EF.Functions.Like(x.Title, $"%{keyword}%"))
                              .Where(x => EF.Functions.Like(x.Markdown, $"%{keyword}%"));

            var query = filter.OrderByDescending(x => x.UpdateTime)
                              .Take(50)
                              .Select(x => new SearchPostDataDto()
                              {
                                  Id = x.Id,
                                  Title = x.Title,
                                  Info = x.Info
                              });

            return await query.ToListAsync();
        }
        #endregion

        #region ======== 管理后台 ========

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<PostQueryDataDto>> GetListAsync(PostQueryFilter input)
        {
            var filter = _post.GetAll()
                              .WhereIf(!input.Title.IsNullOrEmpty(), x => EF.Functions.Like(x.Title, $"%{input.Title!}%"))
                              .WhereIf(input.Category.HasValue, x => x.Category == input.Category!.Value)
                              .WhereIf(input.Type.HasValue, x => x.Type == input.Type!.Value)
                              .WhereIf(input.IsPublish.HasValue, x => x.IsPublish == input.IsPublish!.Value);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<PostQueryDataDto>(count);

            var query = from post in filter.OrderByDescending(x => x.Id).PageBy(input.PageIndex, input.PageSize)

                        join c in _category.GetAll() on post.Category equals c.Id into cc
                        from category in cc.DefaultIfEmpty()

                        join s in _statistics.Value.GetAll() on post.Id equals s.Id into ss
                        from statistics in ss.DefaultIfEmpty()

                        select new PostQueryDataDto()
                        {
                            Id = post.Id,
                            Type = post.Type,
                            Icon = post.Icon,
                            Title = post.Title,
                            Info = post.Info,
                            Category = post.Category,
                            CategoryName = category != null ? category.Name : "未分类",
                            Comment = post.Comment,
                            IsPublish = post.IsPublish,
                            Recommend = post.Recommend,
                            BrowseCount = statistics != null ? statistics.Browse : 0,
                            CommentCount = statistics != null ? statistics.Comment : 0,
                            UpdateTime = post.UpdateTime,
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<PostQueryDataDto>(count, list);
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PostInfoDto> GetInfoAsync(long id)
        {
            var post = await _post.GetAsync(id);
            return post == null
                ? throw new OutputException(ResCodeEnum.DataNotFound, $"post id:{id} not found data")
                : post.ToMap<PostInfoDto>();
        }

        /// <summary>
        /// Markdown插件上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> MarkdownUploadAsync(IFormFile file)
        {
            return await _fileManage.Value.UploadFileAsync(file, "/post/markdown");
        }

        /// <summary>
        /// 保存文章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SaveAsync(PostSaveDto input)
        {
            var data = await _post.GetAsync(input.Id) ?? new Post() { Id = 0, Recommend = false };

            var oldCategory = data.Category;
            if (data.Category != input.Category && input.Category > 0 && !await _category.ExistsAsync(x => x.Id == input.Category!.Value))
                throw new FriendlyException("文章分类不存在，请刷新文章分类选项后重新选择");

            if (input.File != null)
            {
                input.Icon = await _fileManage.Value.UploadFileAsync(input.File, "/post/icon");
            }
            else if (input.Icon.IsNullOrEmpty() && data.Category != input.Category && input.Category > 0)
                input.Icon = await _category.GetSelectAsync(input.Category!.Value, x => x.Icon);

            data = data.Id == 0 ? await CreateAsync(input, data) : await UpdateAsync(input, data);

            _scheduleQueue.Value.Enqueue(ScheduleTypeEnum.CategoryPostCount, oldCategory.ToString());
            _scheduleQueue.Value.Enqueue(ScheduleTypeEnum.CategoryPostCount, data.Category.ToString());
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            var data = await _post.GetAsync(id);
            if (data == null)
                return;

            await _post.DeleteAsync(data);
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task PublishPostAsync(long id)
        {
            var data = await _post.GetAsync(id) ?? throw new FriendlyException("");

            if (data.IsPublish)
                return;

            data.IsPublish = true;
            await _post.UpdateFieIdsAsync(data, x => x.IsPublish);

            _scheduleQueue.Value.Enqueue(ScheduleTypeEnum.CategoryPostCount, data.Category.ToString());
        }

        /// <summary>
        /// 设置文章评论权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task SetPostCommentAsync(long id, bool comment)
        {
            var post = await _post.GetAsync(id) ?? throw new FriendlyException("");

            if (post.Comment == comment)
                return;

            post.Comment = comment;
            await _post.UpdateFieIdsAsync(post, x => x.Comment);
        }

        /// <summary>
        /// 设置文章推荐
        /// </summary>
        /// <param name="id"></param>
        /// <param name="recommend"></param>
        /// <returns></returns>
        public async Task SetPostRecommendAsync(long id, bool recommend)
        {
            if (recommend)
            {
                var count = await _post.GetAll().Where(x => x.IsPublish == true && x.Recommend == true).CountAsync();

                if (count >= 6)
                    throw new FriendlyException("推荐文章最多设置6篇");
            }

            var post = await _post.GetAsync(id) ?? throw new FriendlyException("");

            if (post.Recommend == recommend)
                return;

            post.Recommend = recommend;
            await _post.UpdateFieIdsAsync(post, x => x.Recommend);
        }

        #endregion

        #region ======== 私有方法 ========

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<Post> CreateAsync(PostSaveDto input, Post data)
        {
            data.Title = input.Title!;
            data.Info = input.Info!;
            data.Markdown = input.Markdown!;
            data.Icon = input.Icon ?? "";
            data.Type = input.Type!;
            data.Category = input.Category ?? 0;
            data.Comment = input.Comment ?? false;
            data.Tags = input.Tags.ToJson();
            data.IsPublish = input.IsPublish ?? false;
            data.Recommend = input.Recommend ?? false;
            data.CreateTime = DateTime.Now;
            data.UpdateTime = data.CreateTime;

            if (!data.IsPublish && data.Recommend)
                data.Recommend = false;

            return await _post.CreateAsync(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<Post> UpdateAsync(PostSaveDto input, Post data)
        {
            var fieIds = new List<Expression<Func<Post, object>>>();
            data.UpdatePorpertyIf(!input.Title.IsNullOrEmpty() && input.Title != data.Title, x =>
            {
                x.Title = input.Title!;
                fieIds.Add(x => x.Title!);
            }).UpdatePorpertyIf(input.Info != data.Info, x =>
            {
                x.Info = input.Info!;
                fieIds.Add(x => x.Info!);
            }).UpdatePorpertyIf(input.Markdown != data.Markdown, x =>
            {
                x.Markdown = input.Markdown!;
                fieIds.Add(x => x.Markdown!);
            }).UpdatePorpertyIf(!input.Icon.IsNullOrEmpty() && data.Icon != input.Icon, x =>
            {
                x.Icon = input.Icon!;
                fieIds.Add(x => x.Icon!);
            }).UpdatePorpertyIf(data.Type != input.Type, x =>
            {
                data.Type = input.Type!;
                fieIds.Add(x => x.Type!);
            }).UpdatePorpertyIf(input.Category.HasValue && data.Category != input.Category, x =>
            {
                x.Category = input.Category!.Value;
                fieIds.Add(x => x.Category!);
            }).UpdatePorpertyIf(input.Comment.HasValue && data.Comment != input.Comment, x =>
            {
                data.Comment = input.Comment!.Value;
                fieIds.Add(x => x.Comment!);
            }).UpdatePorpertyIf(input.Tags.HasValue(), x =>
            {
                var tmp = input.Tags.ToJson();
                if (tmp != data.Tags)
                {
                    data.Tags = tmp;
                    fieIds.Add(x => x.Tags!);
                }
            }).UpdatePorpertyIf(input.IsPublish.HasValue && data.IsPublish != input.IsPublish, x =>
            {
                data.IsPublish = input.IsPublish!.Value;
                fieIds.Add(x => x.IsPublish!);
            });

            if (!data.IsPublish && data.Recommend)
            {
                data.Recommend = false;
                fieIds.Add(x => x.Recommend!);
            }

            if (fieIds.Any())
            {
                data.UpdateTime = DateTime.Now;
                fieIds.Add(x => x.UpdateTime!);
                await _post.UpdateFieIdsAsync(data, fieIds);
            }

            return data;
        }

        #endregion
    }
}

