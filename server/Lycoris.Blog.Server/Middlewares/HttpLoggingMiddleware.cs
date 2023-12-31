﻿using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Model.Cnstants;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Lycoris.Blog.Server.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpLoggingMiddleware : BaseMiddleware
    {
        private readonly RequestRouteMap _routeMap;
        private readonly IScheduleQueueCacheService _scheduleQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="factory"></param>
        /// <param name="routeMap"></param>
        /// <param name="scheduleQueue"></param>
        public HttpLoggingMiddleware(RequestDelegate next,
                                     ILycorisLoggerFactory factory,
                                     RequestRouteMap routeMap,
                                     IScheduleQueueCacheService scheduleQueue) : base(next, factory.CreateLogger<HttpLoggingMiddleware>())
        {
            this.IgnoreOpptionsReuqest = true;
            _routeMap = routeMap;
            _scheduleQueue = scheduleQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task InvokeHandlerAsync(HttpContext context)
        {
            // socket 直接过
            if (context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var traceId = context.Items.GetValue(HttpItems.TraceId);
            var httpMethod = context.Request.Method.ToLower();
            var requestIp = context.Items.GetValue(HttpItems.RequestIP);
            var userAgent = context.Items.GetValue(HttpItems.UserAgent);

            var requestHeaders = GetHttpReqeustHeadersMap(context);

            if (IsStaticFileReuqest(context))
            {
                _logger.Info($"{httpMethod} -> {context.Request.Path.Value} - {requestIp}");
                await _next.Invoke(context);
                return;
            }

            // 头部信息记录
            if (requestHeaders.Any())
                _logger.Info($"{httpMethod} -> request headers:[{string.Join(", ", requestHeaders.Select(x => $"{x.Key}:{x.Value}"))}]", traceId);

            var body = await GetHttpRequestBodyAsync(context);
            var path = httpMethod == "get" ? $"{(context.Request.Path.Value ?? "").TrimEnd('/')}{body ?? ""}" : $"{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "")}"; ;

            if (httpMethod == "get")
                _logger.Info($"{httpMethod} -> {path}");
            else
                _logger.Info($"{httpMethod} -> {path}{(body.IsNullOrEmpty() ? "" : $" -> body:{body}")}");

            await _next.Invoke(context);

            var responseLog = new ResponseLogTemp(context)
            {
                HttpMethod = httpMethod,
                Path = path,
                RequestIp = requestIp,
                Request = body ?? "",
                RequestHeaders = requestHeaders,
            };

            context.Response.OnCompleted(() => ResponseOnCompletedAsync(responseLog));

            context.Items.RemoveValue(HttpItems.ResponseBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetHttpReqeustHeadersMap(HttpContext context)
        {
            var headers = new Dictionary<string, string>()
            {
                { HttpHeaders.NginxRemoteIp , ""},
                { HttpHeaders.Host , ""},
                { HttpHeaders.Forwarded , ""},
                { HttpHeaders.UserAgent , ""},
                { HttpHeaders.Authentication , ""}
            };

            if (context.Request.Headers != null)
            {
                headers[HttpHeaders.NginxRemoteIp] = context.Request.Headers.ContainsKey(HttpHeaders.NginxRemoteIp) ? context.Request.Headers[HttpHeaders.NginxRemoteIp]! : "";

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.Host))
                    headers[HttpHeaders.Host] = context.Request.Headers[HttpHeaders.Host].ToString();

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.Forwarded))
                    headers[HttpHeaders.Forwarded] = context.Request.Headers[HttpHeaders.Forwarded]!;

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.UserAgent))
                    headers[HttpHeaders.UserAgent] = context.Request.Headers[HttpHeaders.UserAgent].ToString();

                //
                if (context.Request.Headers.ContainsKey(HttpHeaders.Authentication))
                    headers[HttpHeaders.Authentication] = context.Request.Headers[HttpHeaders.Authentication].ToString();
            }


            return headers.Where(x => !x.Value.IsNullOrEmpty()).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<string?> GetHttpRequestBodyAsync(HttpContext context)
        {
            if (context.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
                return context.Request.QueryString.Value ?? "";

            if (context.Request.Body == null)
                return "";

            if (context.Request.ContentType == null)
                return "";

            try
            {
                context.Request.EnableBuffering();

                var body = "";

                if (context.Request.Body.CanRead)
                {
                    body = await new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();

                    if (context.Request.ContentType.Contains("multipart/form-data"))
                        return await GetFormDataBodyAsync(context, body);

                    body = body.ToJsonString();
                }

                return body ?? "";
            }
            catch (Exception ex)
            {
                _logger.Error($"read request body exception", ex);
                return null;
            }
            finally
            {
                context.Request.Body?.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static async Task<string> GetFormDataBodyAsync(HttpContext context, string body)
        {
            var media = MediaTypeHeaderValue.Parse(context.Request.ContentType);
            var boundary = HeaderUtilities.RemoveQuotes(media.Boundary).Value;

            using var ms = new MemoryStream(Encoding.Default.GetBytes(body));
            var reader = new MultipartReader(boundary!, ms);
            var section = await reader.ReadNextSectionAsync();

            var result = "";

            while (section != null)
            {
                var propName = GetFormDataPropertyName(section.ContentDisposition);

                if (!propName.IsNullOrEmpty())
                {
                    result += $"\"{propName}\":";

                    if (!IsFileSection(section.ContentDisposition))
                    {
                        var propValue = await new StreamReader(section.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();
                        result += $"\"{propValue}\",";
                    }
                    else
                    {
                        result += $"\"file\":\"[File({GetFormDataFileName(section.ContentDisposition)})]\",";
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return result.IsNullOrEmpty() ? "" : $"{{{result.TrimEnd(',')}}}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetHttpResponseHeadersMap(HttpContext context)
        {
            if (context.Response.Headers == null)
                return new Dictionary<string, string>();

            var headers = new Dictionary<string, string>()
            {
                { "Content-Encoding", "" },
                { "ContentType", ""},
                { "StatusCode", ""},
                { "Redirect", ""}
            };

            headers["Content-Encoding"] = context.Response.Headers.ContainsKey("Content-Encoding") ? context.Response.Headers["Content-Encoding"].ToString() : "";
            headers["ContentType"] = context.Response.ContentType;
            headers["StatusCode"] = context.Response.StatusCode.ToString();

            if (context.Response.StatusCode == 302)
                headers["Redirect"] = context.Response.Headers.ContainsKey("Location") ? context.Response.Headers["Location"].ToString() : "";

            return headers.Where(x => !x.Value.IsNullOrEmpty()).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private static string? GetFormDataPropertyName(string? contentDisposition)
        {
            var strArray = contentDisposition?.Split(';');
            var formData = strArray?.FirstOrDefault(part => part.Contains("name="));
            return formData?.Split('=').Last().Trim('"') ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private static string? GetFormDataFileName(string? contentDisposition)
        {
            var strArray = contentDisposition?.Split(';');
            var formData = strArray?.FirstOrDefault(part => part.Contains("filename="));
            return formData?.Split('=').Last().Trim('"') ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        private static bool IsFileSection(string? contentDisposition) => contentDisposition?.Split(';')?.FirstOrDefault(part => part.Contains("filename="))?.Any() ?? false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logTmp"></param>
        /// <returns></returns>
        private Task ResponseOnCompletedAsync(ResponseLogTemp logTmp)
        {
            var log = new StringBuilder();
            var temp = logTmp.ResponseHeaders.Select(x => $"{x.Key}: {x.Value}").ToArray();
            log.AppendFormat("{0} -> response headers:[{1}]", logTmp.HttpMethod, string.Join("; ", temp));
            _logger.Info(log.ToString());

            log.Clear();

            if (!logTmp.Response.IsNullOrEmpty())
            {
                log.AppendFormat("{0} -> response body", logTmp.HttpMethod);
                log.Append(" - ");
                log.Append(logTmp.Response);
            }
            else
                log.AppendFormat("{0} response", logTmp.HttpMethod);

            log.AppendFormat(" - {0} - ", logTmp.StatusCode);

            var elapsedMilliseconds = (DateTime.Now - logTmp.RequestTime).TotalMilliseconds;

            log.AppendFormat("{0}ms", elapsedMilliseconds.ToString("0.000"));
            _logger.Info(log.ToString());

            RequestLog(new RequestLogQueueModel()
            {
                Method = logTmp.HttpMethod.ToUpper(),
                Route = logTmp.Path,
                Headers = logTmp.RequestHeaders,
                Params = logTmp.HttpMethod == "get" ? "" : (logTmp.Request ?? ""),
                StatusCode = logTmp.StatusCode,
                Response = logTmp.Response ?? "",
                ElapsedMilliseconds = (long)elapsedMilliseconds,
                Ip = logTmp.RequestIp,
                Exception = logTmp.Exception,
                StackTrace = logTmp.StackTrace,
                CreateTime = logTmp.RequestTime
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void RequestLog(RequestLogQueueModel data)
        {
            if (_routeMap.LogFilter == RequestLogFilterEnum.OnlyWeb && !_routeMap.WebRoute.Any(x => x.Equals(data.Route, StringComparison.CurrentCultureIgnoreCase)))
                return;

            if (_routeMap.LogFilter == RequestLogFilterEnum.OnlyDashboard && !_routeMap.DashboardRoute.Any(x => x.Equals(data.Route, StringComparison.CurrentCultureIgnoreCase)))
                return;

            _scheduleQueue.Enqueue(ScheduleTypeEnum.RequestLog, data);
        }

        /// <summary>
        /// 
        /// </summary>
        private class ResponseLogTemp
        {
            public ResponseLogTemp(HttpContext context)
            {
                this.ResponseHeaders = GetHttpResponseHeadersMap(context);
                this.Response = context.Items.GetValue(HttpItems.ResponseBody);
                this.StatusCode = context.Response.StatusCode;
                this.RequestTime = context.Items.GetValue<DateTime>(HttpItems.RequestTime);
                this.Exception = context.Items.GetValue(HttpItems.Exception) ?? "";
                this.StackTrace = context.Items.GetValue(HttpItems.StackTrace) ?? "";
            }

            public string HttpMethod { get; set; } = string.Empty;

            public string Path { get; set; } = string.Empty;

            public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

            public DateTime RequestTime { get; set; }

            public Dictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();

            public string Request { get; set; } = string.Empty;

            public string Response { get; set; } = string.Empty;

            public int StatusCode { get; set; }

            public string RequestIp { get; set; } = string.Empty;

            public string Exception { get; set; } = string.Empty;

            public string StackTrace { get; set; } = string.Empty;
        }
    }
}
