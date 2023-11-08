using Lycoris.Blog.Core.Logging;
using Lycoris.Quartz.Extensions;

namespace Lycoris.Blog.Application.Schedule.Shared
{
    public abstract class BaseJob : BaseQuartzJob
    {
        protected readonly JobLogger _logger;

        public BaseJob(ILycorisLogger logger)
        {
            _logger = new JobLogger(logger);
        }


        protected override Task DoWorkAsync()
        {
            _logger.JobWorkRegister(JobTraceId, JobName);
            try
            {
                return HandlerWorkAsync();
            }
            catch (Exception ex)
            {
                this._logger.Error("job handle failed", ex);
                return Task.CompletedTask;
            }
        }

        protected abstract Task HandlerWorkAsync();
    }
}
