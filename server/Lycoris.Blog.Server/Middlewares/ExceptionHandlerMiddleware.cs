using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
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

            if (IsStaticFileReuqest(context))
            {
                var referer = context.Request.Headers.ContainsKey(HttpHeaders.Referer) ? context.Request.Headers[HttpHeaders.Referer].ToString() : "";

                // 处理防盗链
                if (referer.IsNullOrEmpty() || !AppSettings.Application.Cors.Origins.Any(x => referer.Contains(x)))
                {
                    context.Response.StatusCode = 404;
                    _logger.Error($"static file request path '{context.Request.Path.Value}' referer:{referer} is not match origins:{string.Join(",", AppSettings.Application.Cors.Origins)}");
                    return;
                }
            }
            else if (!ApiRequestVerification(context, startTime, ipAddress, traceId))
                return;

            try
            {
                context.Items.AddOrUpdate(HttpItems.TraceId, traceId);
                context.Items.AddOrUpdate(HttpItems.RequestTime, DateTime.Now);

                var userAgent = context.Request.Headers.ContainsKey(HttpHeaders.UserAgent) ? context.Request.Headers[HttpHeaders.UserAgent].ToString() : "";
                context.Items.AddOrUpdate(HttpItems.UserAgent, userAgent);

                // 执行下一个中间件
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var httpMethod = context.Request.Method.ToUpper();
                var path = context.Request.Path.Value ?? "";
                var queryString = context.Request.QueryString.Value ?? "";
                var statusCode = context.Response.StatusCode;

                RequestLog(httpMethod, $"{path.TrimEnd('/')}?{queryString.TrimStart('?')}", "", statusCode, startTime, ipAddress, ex);
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
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="startTime"></param>
        /// <param name="ipAddress"></param>
        /// <param name="traceId"></param>
        /// <returns></returns>
        private bool ApiRequestVerification(HttpContext context, DateTime startTime, string ipAddress, string traceId)
        {
            var result = true;
            var response = "";

            if (!CheckAllowMethod(context))
            {
                response = "request method invalid";
                context.Response.StatusCode = 415;
                _logger.Error($"invalid request - {(context.Request.Path.HasValue ? context.Request.Path.Value : "/")} - {response}", traceId);
                result = false;
            }
            else if (!CheckAllowRoute(context))
            {
                response = "request path invalid";
                context.Response.StatusCode = 400;
                _logger.Error($"invalid request - {(context.Request.Path.HasValue ? context.Request.Path.Value : "/")} - {response}", traceId);
                result = false;
            }
            else if (!CheckRequestOrign(context))
            {
                response = "request orign invalid";
                context.Response.StatusCode = 400;
                _logger.Error($"invalid request - {(context.Request.Path.HasValue ? context.Request.Path.Value : "/")} - {response}", traceId);
                result = false;
            }

            if (!result)
            {
                var httpMethod = context.Request.Method.ToUpper();
                var path = context.Request.Path.Value ?? "";
                var queryString = context.Request.QueryString.Value ?? "";
                var statusCode = context.Response.StatusCode;

                context.Response.OnCompleted(() =>
                {
                    RequestLog(httpMethod, $"{path.TrimEnd('/')}?{queryString.TrimStart('?')}", response, statusCode, startTime, ipAddress);
                    return Task.CompletedTask;
                });
            }

            return result;
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
            string orign = "";

            if (context.Request.Headers.ContainsKey(HttpHeaders.Origin))
                orign = context.Request.Headers[HttpHeaders.Origin].ToString();

            if (!orign.IsNullOrEmpty() && AppSettings.Application.Cors.Origins.HasValue() && !AppSettings.Application.Cors.Origins.Contains(orign))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="path"></param>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <param name="startTime"></param>
        /// <param name="ipAddress"></param>
        /// <param name="ex"></param>
        private void RequestLog(string httpMethod, string path, string response, int statusCode, DateTime startTime, string ipAddress, Exception? ex = null)
        {
            _scheduleQueue.Enqueue(ScheduleTypeEnum.RequestLog, new RequestLogQueueModel()
            {
                Method = httpMethod,
                Route = path,
                Params = "",
                StatusCode = statusCode,
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
