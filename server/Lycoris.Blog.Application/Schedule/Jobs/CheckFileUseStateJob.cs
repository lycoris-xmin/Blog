﻿using Lycoris.Blog.Application.Cached.StaticFiles;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Application.SignalR.Hubs;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    [QuartzJob("检测文件归档", Standby = true)]
    public class CheckFileUseStateJob : BaseJob
    {
        private readonly IRepository<StaticFile, long> _staticFile;
        private readonly IRepository<Post, long> _post; // 60
        private readonly IRepository<Category, int> _category; // 20
        private readonly IRepository<WebSiteAbout, string> _webSiteAbout; // 20
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly Lazy<IStaticFilesCacheService> _cache;

        public CheckFileUseStateJob(ILycorisLoggerFactory factory,
                                    IRepository<StaticFile, long> staticFile,
                                    IRepository<Post, long> post,
                                    IRepository<Category, int> category,
                                    IRepository<WebSiteAbout, string> webSiteAbout,
                                    IHubContext<DashboardHub> hubContext,
                                    Lazy<IStaticFilesCacheService> cache) : base(factory.CreateLogger<CheckFileUseStateJob>())
        {
            _staticFile = staticFile;
            _post = post;
            _category = category;
            _webSiteAbout = webSiteAbout;
            _hubContext = hubContext;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            var args = this.Context.GetJobArgs();
            if (args.IsNullOrEmpty())
            {
                // 
                return;
            }

            var fileId = args.ToLong();

            var file = await _staticFile.GetAsync(fileId);

            if (file == null)
            {
                // 
                return;
            }

            var result = await CheckPostUseAsync(file);
            result = await CheckCategoryUseAsync(file, result);
            result = await CheckWebSiteAboutUseAsync(file, result);

            // 更新数据库
            if (file.Use != result.Use)
            {
                file.Use = result.Use;
                await _staticFile.UpdateFieIdsAsync(file, x => x.Use);
            }

            // 推送
            await _hubContext.Clients.All.SendAsync("checkkFileUseState", new
            {
                Id = fileId.ToString(),
                file.Use,
                result.Message
            });

            _cache.Value.RemoveStaticFileUse(file.FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<CheckResult> CheckPostUseAsync(StaticFile file)
        {
            var pageIndex = 1;
            var pageSize = 20;

            do
            {
                var postList = await _post.GetAll().PageBy(pageIndex, pageSize).Select(x => new { x.Title, x.Icon, x.Markdown }).ToListAsync();
                if (!postList.HasValue())
                    break;

                // 
                foreach (var item in postList)
                {
                    if (item.Icon == file.PathUrl || item.Markdown.IndexOf(file.PathUrl) > -1)
                        return new CheckResult($"文章 {item.Title} 使用中");
                }

                if (postList.Count < pageSize)
                    break;

                pageIndex++;

            } while (true);

            return new CheckResult(); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<CheckResult> CheckCategoryUseAsync(StaticFile file, CheckResult result)
        {
            if (result.Use)
                return result;

            var list = await _category.GetAll().Select(x => new { x.Name, x.Icon }).ToListAsync();

            foreach (var item in list)
            {
                if (item.Icon == file.PathUrl)
                {
                    result.Use = true;
                    result.Message = $"分类 {item.Name} 使用中";
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<CheckResult> CheckWebSiteAboutUseAsync(StaticFile file, CheckResult result)
        {
            if (result.Use)
                return result;

            var data = await _webSiteAbout.GetAsync(AppAbout.AboutWeb);

            result.Use = data!.Value.IndexOf(file.PathUrl) > -1;
            if (result.Use)
                result.Message = "关于本站 使用中";

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private class CheckResult
        {
            public CheckResult()
            {
                this.Use = false;
            }

            public CheckResult(string Message)
            {
                this.Use = true;
                this.Message = Message;
            }

            /// <summary>
            /// 
            /// </summary>
            public bool Use { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string? Message { get; set; }
        }
    }
}
