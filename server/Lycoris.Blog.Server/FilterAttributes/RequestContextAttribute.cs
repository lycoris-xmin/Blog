using Lycoris.Blog.Common.Extensions;
using Lycoris.Blog.Model.Cnstants;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lycoris.Blog.Server.FilterAttributes
{
    /// <summary>
    /// 请求上下文处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequestContextAttribute : BaseActionAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ActionHandlerBeforeAsync(ActionExecutingContext context)
        {
            var request = context.HttpContext.GetService<RequestContext>();

            // 请求唯一标识码
            request.TraceId = context.HttpContext.Items.GetValue(HttpItems.TraceId);

            // 请求Ip
            request.RequestIP = context.HttpContext.Items.GetValue(HttpItems.RequestIP);
            context.HttpContext.Items.Remove(HttpItems.RequestIP);

            // 请求终端
            request.UserAgent = context.HttpContext.Items.GetValue(HttpItems.UserAgent);

            // 访问令牌
            request.Token = context.HttpContext.Request.Headers.ContainsKey(HttpHeaders.Authentication)
                            ? context.HttpContext.Request.Headers[HttpHeaders.Authentication].ToString()
                            : "";

            return Task.CompletedTask;
        }
    }
}
