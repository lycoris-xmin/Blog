using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    public class RequestLogQueueModel
    {
        public RequestLogQueueModel()
        {

        }

        public RequestLogQueueModel(HttpContext context)
        {
            this.Method = context.Request.Method.ToUpper();
            this.Params = context.Request.QueryString.Value ?? "";
            this.Route = context.Request.Path.Value ?? "";
            this.StatusCode = context.Response.StatusCode;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 请求路由
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

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
        /// 客户端Ip
        /// </summary>
        public string Ip { get; set; } = string.Empty;

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
