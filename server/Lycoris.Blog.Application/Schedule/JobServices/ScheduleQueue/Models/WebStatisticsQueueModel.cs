namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class WebStatisticsQueueModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Browse { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int CommentMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int User { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string? UserAgent { get; set; }
    }
}
