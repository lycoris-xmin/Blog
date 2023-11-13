namespace Lycoris.Blog.Server.Models.AccessControls
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessControlLogDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 请求路由
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string? Params { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public uint StatusCode { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        public string? Response { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        public string? StackTrace { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
