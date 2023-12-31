﻿namespace Lycoris.Blog.Server.Models.RequestLogs
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestLogInfoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public uint StatusCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string? Params { get; set; }

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
    }
}
