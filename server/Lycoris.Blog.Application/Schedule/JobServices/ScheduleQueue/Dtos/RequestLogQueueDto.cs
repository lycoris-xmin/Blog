namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos
{
    public class RequestLogQueueDto
    {
        /// <summary>
        /// 请求方式
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 请求路由
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Params { get; set; } = string.Empty;

        /// <summary>
        /// 响应状态吗
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// 耗时
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception { get; set; } = string.Empty;

        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string IP { get; set; } = string.Empty;

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
