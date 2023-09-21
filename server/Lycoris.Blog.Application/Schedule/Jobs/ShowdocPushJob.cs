using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Core.Showdoc;
using Lycoris.Quartz.Extensions.Job;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("Showdoc公众号推送", Trigger = QuartzTriggerEnum.SIMPLE, IntervalSecond = 5)]
    public class ShowdocPushJob : BaseJob
    {
        private readonly IShowdocService _showdoc;

        public ShowdocPushJob(ILycorisLoggerFactory factory, IShowdocService showdoc) : base(factory.CreateLogger<ShowdocPushJob>())
        {
            _showdoc = showdoc;
        }

        protected override Task HandlerWorkAsync(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
