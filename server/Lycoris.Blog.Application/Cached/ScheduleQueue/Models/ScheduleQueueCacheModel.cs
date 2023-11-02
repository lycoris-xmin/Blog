namespace Lycoris.Blog.Application.Cached.ScheduleQueue.Models
{
    public class ScheduleQueueCacheModel
    {
        public ScheduleTypeEnum Type { get; set; }

        public string? Data { get; set; }

        public DateTime? Time { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ScheduleTypeEnum
    {
        /// <summary>
        /// 分类文章异步任务
        /// </summary>
        CategoryPostCount = 0,
        /// <summary>
        /// 请求日志异步任务
        /// </summary>
        RequestLog = 1,
        /// <summary>
        /// 网站浏览日志
        /// </summary>
        BrowseLog = 2,
        /// <summary>
        /// 文章异步任务
        /// </summary>
        PostStatistics = 3,
        /// <summary>
        /// 留言异步任务
        /// </summary>
        LeaveMessage = 4,
        /// <summary>
        /// 用户登录记录
        /// </summary>
        UserLoginRecord = 5,
        /// <summary>
        /// 帐号登录失败记录
        /// </summary>
        LoginFailedRecord = 6
    }
}
