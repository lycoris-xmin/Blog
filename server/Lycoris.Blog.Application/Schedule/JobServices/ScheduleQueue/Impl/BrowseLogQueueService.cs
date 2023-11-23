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
using System.Text.RegularExpressions;

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

        private readonly IRepository<BrowseReferer, int> _browseReferer;
        private readonly IScheduleQueueCacheService _scheduleQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browseReferer"></param>
        /// <param name="scheduleQueue"></param>
        public BrowseLogQueueService(IRepository<BrowseReferer, int> browseReferer, IScheduleQueueCacheService scheduleQueue)
        {
            _browseReferer = browseReferer;
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

            // referer 统计
            if (!model.Referer.IsNullOrEmpty())
            {
                var referer = await _browseReferer.GetAll().Where(x => x.Referer == model.Referer).SingleOrDefaultAsync() ?? new BrowseReferer() { Referer = model.Referer!, Count = 0, Domain = GetUrlDomain(model.Referer!) };

                referer.Count++;

                await _browseReferer.CreateOrUpdateAsync(referer, x => x.Count);
            }

            var queueModel = new WebStatisticsQueueModel() { Browse = 1 };

            if (!model.Ip.IsNullOrEmpty())
            {
                var addr = IPAddressHelper.Search(model.Ip!);
                queueModel.Country = addr.IsPrivate ? "中国" : addr.Country ?? "";
            }

            // 
            _scheduleQueue.Enqueue(ScheduleTypeEnum.WebStatistics, queueModel);
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
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetUrlDomain(string url)
        {
            // 使用正则表达式从URL中提取http://或https://及后面的域名部分
            string pattern = @"^(https?:\/\/[^\/]+)";
            var match = Regex.Match(url, pattern);

            if (match.Success)
                return match.Groups[1].Value;

            return string.Empty;
        }
    }
}
