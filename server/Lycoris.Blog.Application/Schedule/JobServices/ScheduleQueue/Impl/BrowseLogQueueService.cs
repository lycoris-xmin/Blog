using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "BrowseLog")]
    public class BrowseLogQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }
        public JobLogger? JobLogger { get; set; }

        private readonly IRepository<BrowseWorldMap, int> _browseWordMap;
        private readonly IRepository<BrowseStatistics, int> _browseStatistics;
        private readonly IRepository<RefererStatistics, int> _refererStatistics;
        private readonly IScheduleQueueCacheService _scheduleQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browseReferer"></param>
        /// <param name="browseWordMap"></param>
        /// <param name="browseStatistics"></param>
        /// <param name="refererStatistics"></param>
        /// <param name="scheduleQueue"></param>
        public BrowseLogQueueService(IRepository<BrowseWorldMap, int> browseWordMap,
                                     IRepository<BrowseStatistics, int> browseStatistics,
                                     IRepository<RefererStatistics, int> refererStatistics,
                                     IScheduleQueueCacheService scheduleQueue)
        {
            _browseWordMap = browseWordMap;
            _browseStatistics = browseStatistics;
            _refererStatistics = refererStatistics;
            _scheduleQueue = scheduleQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var model = data?.ToObject<BrowseLogQueueModel>();
            if (model == null)
            {
                this.JobLogger!.Error($"can not find any data");
                return;
            }

            // 文章浏览量统计事件插入
            PushCalcPostStatisticsQueue(model);

            // 浏览分布处理
            await HandleBrowseMapAsync(model);

            // 网站浏览统计
            await HandlerBrowseStatisticsAsync(model);

            // 来源域名统计
            await HandleRefererStatisticsAsync(model);

            // 插入网站总互动统计队列
            _scheduleQueue.Enqueue(ScheduleTypeEnum.WebStatistics, new WebStatisticsQueueModel() { Browse = 1, UserAgent = model.UserAgent });
        }

        /// <summary>
        /// 文章浏览量统计事件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void PushCalcPostStatisticsQueue(BrowseLogQueueModel data)
        {
            try
            {
                if (!data.Path.StartsWith("/post", StringComparison.CurrentCultureIgnoreCase))
                    return;

                var postId = (data.Path.TrimEnd('/').Split('/').LastOrDefault() ?? "").ToTryLong();
                if (!postId.HasValue || postId.Value <= 0)
                    return;

                _scheduleQueue.Enqueue(ScheduleTypeEnum.PostStatistics, new PostStaticQueueModel(postId!.Value, PostStaticTypeEnum.Browse));
            }
            catch (Exception ex)
            {
                this.JobLogger!.Warn("", ex);
            }
        }

        /// <summary>
        /// 浏览分布处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task HandleBrowseMapAsync(BrowseLogQueueModel model)
        {
            if (!model.Ip.IsNullOrEmpty())
            {
                var addr = IPAddressHelper.Search(model.Ip!);
                var country = addr.IsPrivate ? "中国" : addr.Country ?? "";

                if (!country.IsNullOrEmpty())
                {
                    var map = await _browseWordMap.GetAll().Where(x => x.Country == country).SingleOrDefaultAsync() ?? new BrowseWorldMap() { Country = country, Count = 0 };
                    if (map.Count < int.MaxValue)
                    {
                        map.Count++;
                        await _browseWordMap.CreateOrUpdateAsync(map, x => x.Count);
                    }
                }
            }
        }

        /// <summary>
        /// 网站浏览统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task HandlerBrowseStatisticsAsync(BrowseLogQueueModel model)
        {
            try
            {
                var data = await _browseStatistics.GetAll().Where(x => x.Route == model.Path).SingleOrDefaultAsync() ?? new BrowseStatistics() { Route = model.Path, PageName = model.PageName, Count = 0 };
                data.Count++;
                await _browseStatistics.CreateOrUpdateAsync(data, x => x.Count);
            }
            catch (Exception ex)
            {
                this.JobLogger!.Error("handle browse statistics failed", ex);
            }
        }

        /// <summary>
        /// 来源域名统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task HandleRefererStatisticsAsync(BrowseLogQueueModel model)
        {
            try
            {
                if (model.Referer.IsNullOrEmpty())
                    return;

                var domain = UrlHelper.GetUrlPrefix(model.Referer!).Replace("https://", "").Replace("http://", "");
                var data = await _refererStatistics.GetAll().Where(x => x.Domain == domain).SingleOrDefaultAsync() ?? new RefererStatistics() { Referer = model.Referer!, Domain = domain, Count = 0 };
                data.Count++;
                await _refererStatistics.CreateOrUpdateAsync(data, x => x.Count);
            }
            catch (Exception ex)
            {
                this.JobLogger!.Error("handle referer statistics failed", ex);
            }
        }
    }
}
