using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue
{
    public interface IScheduleQueueService
    {
        /// <summary>
        /// 
        /// </summary>
        IJobExecutionContext? JobContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Task JobDoWorkAsync(string? data, DateTime? time);
    }
}
