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

        public CheckFileUseStateJob(ILycorisLoggerFactory factory,
                                    IRepository<StaticFile, long> staticFile,
                                    IRepository<Post, long> post,
                                    IRepository<Category, int> category,
                                    IRepository<WebSiteAbout, string> webSiteAbout,
                                    IHubContext<DashboardHub> hubContext) : base(factory.CreateLogger<CheckFileUseStateJob>())
        {
            _staticFile = staticFile;
            _post = post;
            _category = category;
            _webSiteAbout = webSiteAbout;
            _hubContext = hubContext;
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
            if (file.Use != result)
            {
                file.Use = result;
                await _staticFile.UpdateFieIdsAsync(file, x => x.Use);
            }

            // 推送
            await _hubContext.Clients.All.SendAsync("checkkFileUseState", new
            {
                Id = fileId.ToString(),
                file.Use
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<bool> CheckPostUseAsync(StaticFile file)
        {
            var pageIndex = 1;
            var pageSize = 20;

            do
            {
                var markdowns = await _post.GetAll().PageBy(pageIndex, pageSize).Select(x => new { x.Icon, x.Markdown }).ToListAsync();
                if (!markdowns.HasValue())
                    break;

                // 
                foreach (var item in markdowns)
                {
                    if (item.Icon == file.PathUrl || item.Markdown.IndexOf(file.PathUrl) > -1)
                        return true;
                }

                if (markdowns.Count < pageSize)
                    break;

                pageIndex++;

            } while (true);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<bool> CheckCategoryUseAsync(StaticFile file, bool result)
        {
            if (result)
                return result;

            var list = await _category.GetAll().Select(x => x.Icon).ToListAsync();

            foreach (var item in list)
            {
                if (item == file.PathUrl)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<bool> CheckWebSiteAboutUseAsync(StaticFile file, bool result)
        {
            if (result)
                return result;

            var data = await _webSiteAbout.GetAsync(AppAbout.AboutWeb);

            return data!.Value.IndexOf(file.PathUrl) > -1;
        }
    }
}
