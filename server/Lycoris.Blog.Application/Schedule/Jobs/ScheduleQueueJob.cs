using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Cache.ScheduleQueue;
using Lycoris.Blog.Core.Logging;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions.Job;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("任务队列", Trigger = QuartzTriggerEnum.SIMPLE, IntervalSecond = 1)]
    public class ScheduleQueueJob : BaseJob
    {
        private readonly IAutofacMultipleService _multipleService;
        private readonly IScheduleQueueCacheService _queueCacheService;

        public ScheduleQueueJob(ILycorisLoggerFactory factory, IAutofacMultipleService multipleService, IScheduleQueueCacheService queueCacheService) : base(factory.CreateLogger<ScheduleQueueJob>())
        {
            _multipleService = multipleService;
            _queueCacheService = queueCacheService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            var cache = _queueCacheService.Dequeue();
            if (cache == null || cache.Data.IsNullOrEmpty())
                return;

            var sechduleJob = _multipleService.TryGetService<IScheduleQueueService>(cache.Type.ToString());
            if (sechduleJob != null)
            {
                sechduleJob.JobContext = Context;
                await sechduleJob.JobDoWorkAsync(cache!.Data, cache.Time);
            }
        }
    }
}
