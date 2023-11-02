using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
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

        private readonly ILycorisLogger _logger;
        private readonly IRepository<BrowseReferer, int> _browseReferer;
        private readonly IScheduleQueueCacheService _scheduleQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="browseReferer"></param>
        /// <param name="scheduleQueue"></param>
        public BrowseLogQueueService(ILycorisLoggerFactory factory,
                                   IRepository<BrowseReferer, int> browseReferer,
                                   IScheduleQueueCacheService scheduleQueue)
        {
            _logger = factory.CreateLogger<BrowseLogQueueService>();
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
            var dto = data?.ToObject<BrowseLogQueueModel>();
            if (dto == null)
                return;

            // 文章浏览量统计事件插入
            await CalcPostStatisticsAsync(dto);

            if (dto.Referer.IsNullOrEmpty())
                return;

            var referer = await _browseReferer.GetAll().Where(x => x.Referer == dto.Referer).SingleOrDefaultAsync() ?? new BrowseReferer() { Referer = dto.Referer!, Count = 0, Domain = GetUrlDomain(dto.Path) };

            referer.Count++;

            await _browseReferer.CreateOrUpdateAsync(referer, x => x.Count);
        }

        /// <summary>
        /// 文章浏览量统计事件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task CalcPostStatisticsAsync(BrowseLogQueueModel data)
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
                _logger.Warn("", ex);
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
            Match match = Regex.Match(url, pattern);

            if (match.Success)
                return match.Groups[1].Value;

            return string.Empty;
        }
    }
}
