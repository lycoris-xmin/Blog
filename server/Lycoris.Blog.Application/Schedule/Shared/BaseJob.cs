using Lycoris.Blog.Core.Logging;
using Lycoris.Quartz.Extensions;

namespace Lycoris.Blog.Application.Schedule.Shared
{
    public abstract class BaseJob : BaseQuartzJob
    {
        protected readonly JobLogger JobLogger;

        public BaseJob(ILycorisLogger logger)
        {
            JobLogger = new JobLogger(logger);
        }


        protected override Task DoWorkAsync()
        {
            this.JobLogger.JobWorkRegister(JobTraceId, JobName);
            try
            {
                return HandlerWorkAsync();
            }
            catch (Exception ex)
            {
                this.JobLogger.Error("job handle failed", ex);
                return Task.CompletedTask;
            }
        }

        protected abstract Task HandlerWorkAsync();
    }
}
