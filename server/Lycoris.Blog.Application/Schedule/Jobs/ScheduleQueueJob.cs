using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Logging;
using Lycoris.Blog.Application.Cached.ScheduleQueueCache;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue;
using Lycoris.Blog.Application.Schedule.Shared;
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
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task DoWorkAsync(IJobExecutionContext context)
        {
            var cache = await _queueCacheService.DequeueAsync();
            if (cache == null || cache.Data.IsNullOrEmpty())
                return;

            var sechduleJob = _multipleService.TryGetService<IScheduleQueueService>(cache.Type.ToString());
            if (sechduleJob != null)
            {
                sechduleJob.JobContext = context;
                await sechduleJob.JobDoWorkAsync(cache!.Data, cache.Time);
            }
        }
    }
}
