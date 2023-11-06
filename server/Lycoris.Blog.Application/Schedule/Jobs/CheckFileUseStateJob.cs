using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Core.Logging;
using Lycoris.Quartz.Extensions;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    [QuartzJob("检测文件归档", Standby = true)]
    public class CheckFileUseStateJob : BaseJob
    {
        public CheckFileUseStateJob(ILycorisLoggerFactory factory) : base(factory.CreateLogger<CheckFileUseStateJob>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Task HandlerWorkAsync()
        {
            var args = this.Context.GetJobArgs();
            Console.WriteLine(args);
            return Task.CompletedTask;
        }
    }
}
