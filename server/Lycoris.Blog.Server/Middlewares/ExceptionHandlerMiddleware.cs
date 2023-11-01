﻿using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Cache.ScheduleQueue;
using Lycoris.Blog.Cache.ScheduleQueue.Models;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Model.Cnstants;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;

namespace Lycoris.Blog.Server.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionHandlerMiddleware : BaseMiddleware
    {
        private readonly IScheduleQueueCacheService _scheduleQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="factory"></param>
        /// <param name="scheduleQueue"></param>
        public ExceptionHandlerMiddleware(RequestDelegate next,
                                          ILycorisLoggerFactory factory,
                                          IScheduleQueueCacheService scheduleQueue) : base(next, factory.CreateLogger<ExceptionHandlerMiddleware>())
        {
            this.IgnoreOpptionsReuqest = true;
            _scheduleQueue = scheduleQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task InvokeHandlerAsync(HttpContext context)
        {
            var startTime = DateTime.Now;
            var traceId = Guid.NewGuid().ToString("N");

            var ipAddress = GetRequestIpAddress(context);
            context.Items.AddOrUpdate(HttpItems.RequestIP, ipAddress);

            if (!CheckAllowMethod(context))
            {
                context.Response.StatusCode = 415;
                RequestLog(context, "request method invalid", startTime, ipAddress);
                _logger.Error($"invalid request - {(context.Request.Path.HasValue ? context.Request.Path.Value : "/")} - request method invalid", traceId);
                return;
            }

            if (!CheckAllowRoute(context))
            {
                context.Response.StatusCode = 400;
                RequestLog(context, "request path invalid", startTime, ipAddress);
                _logger.Error($"invalid request - {(context.Request.Path.HasValue ? context.Request.Path.Value : "/")} - request path invalid", traceId);
                return;
            }

            if (!CheckRequestOrign(context))
            {
                context.Response.StatusCode = 400;
                RequestLog(context, "request orign invalid", startTime, ipAddress);
                _logger.Error($"invalid request - {(context.Request.Path.HasValue ? context.Request.Path.Value : "/")} - request orign invalid", traceId);
                return;
            }

            context.Items.AddOrUpdate(HttpItems.TraceId, traceId);
            context.Items.AddOrUpdate(HttpItems.RequestTime, DateTime.Now);

            try
            {
                var userAgent = context.Request.Headers.ContainsKey(HttpHeaders.UserAgent) ? context.Request.Headers[HttpHeaders.UserAgent].ToString() : "";
                context.Items.AddOrUpdate(HttpItems.UserAgent, userAgent);

                // 执行下一个中间件
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                RequestLog(context, "", startTime, ipAddress, ex);
                await HandleWebApiExceptionAsync(context, ex, traceId, startTime);
            }
        }

        /// <summary>
        /// 获取请求来源IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetRequestIpAddress(HttpContext context)
        {
            try
            {
                string ipAddress = string.Empty;

                // 网关转发的客户端请求IP
                if (context.Request.Headers.ContainsKey(HttpHeaders.RequestIP))
                    ipAddress = context.Request.Headers[HttpHeaders.RequestIP].ToString();

                // 代理转发请求头
                if (ipAddress.IsNullOrEmpty() && context.Request.Headers.ContainsKey(HttpHeaders.Forwarded))
                {
                    var proxys = context.Request.Headers[HttpHeaders.Forwarded].ToString();
                    if (!proxys.IsNullOrEmpty())
                        ipAddress = proxys!.Split(',').Where(x => x.IsNullOrEmpty() == false && x.ToLower() != "unknown").FirstOrDefault() ?? "";
                }

                // Nginx转发请求头
                if (ipAddress.IsNullOrEmpty() && context.Request.Headers.ContainsKey(HttpHeaders.NginxRemoteIp))
                    ipAddress = context.Request.Headers[HttpHeaders.NginxRemoteIp]!;

                if (ipAddress.IsNullOrEmpty())
                    ipAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";

                if (ipAddress == "0.0.0.1" || ipAddress == "127.0.0.1" || ipAddress == "localhost")
                    ipAddress = "127.0.0.1";

                return ipAddress;
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
                return "";
            }
        }

        /// <summary>
        /// WebApi全局错误拦截
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="traceId"></param>
        /// <param name="requestTime"></param>
        /// <returns></returns>
        private async Task HandleWebApiExceptionAsync(HttpContext context, Exception ex, string traceId, DateTime requestTime)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";

            if (AppSettings.IsDebugger)
            {
                var res = new BaseOutput
                {
                    ResCode = ResCodeEnum.ApplicationError,
                    ResMsg = ex?.Message ?? "",
                    TraceId = traceId
                };

                await context.Response.WriteAsync(res.ToJson());
            }

            _logger.Error($"global middleware {ex?.GetType().Name ?? "exception"} catch - {(DateTime.Now - requestTime).TotalMilliseconds:0.000}ms", ex, traceId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool CheckAllowMethod(HttpContext context)
            => context.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase) || context.Request.Method.Equals("POST", StringComparison.CurrentCultureIgnoreCase) || context.Request.Method.Equals("OPTIONS", StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool CheckAllowRoute(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            if (path == "" || path == "/")
                return false;

            if (!path.StartsWith($"/{HostConstant.RoutePrefix}", StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool CheckRequestOrign(HttpContext context)
        {
            string orign = "", referer = "";

            if (context.Request.Headers.ContainsKey(HttpHeaders.Origin))
                orign = context.Request.Headers[HttpHeaders.Origin].ToString();

            if (context.Request.Headers.ContainsKey(HttpHeaders.Referer))
                referer = context.Request.Headers[HttpHeaders.Referer].ToString();

            if (!orign.IsNullOrEmpty() && AppSettings.Application.Cors.Origins.HasValue() && !AppSettings.Application.Cors.Origins.Contains(orign))
                return false;

            context.Items.AddOrUpdate(HttpItems.Origin, orign);
            context.Items.AddOrUpdate(HttpItems.Referer, referer);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response"></param>
        /// <param name="startTime"></param>
        /// <param name="ipAddress"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private void RequestLog(HttpContext context, string response, DateTime startTime, string ipAddress, Exception? ex = null)
        {
            _scheduleQueue.Enqueue(ScheduleTypeEnum.RequestLog, new RequestLogQueueModel()
            {
                Method = context.Request.Method.ToUpper(),
                Route = context.Request.Path,
                Params = "",
                StatusCode = context.Response.StatusCode,
                Response = response,
                ElapsedMilliseconds = (long)((DateTime.Now - startTime).TotalMilliseconds),
                IP = ipAddress,
                Exception = ex?.Message ?? "",
                StackTrace = ex?.StackTrace ?? "",
                CreateTime = startTime
            });
        }
    }
}
