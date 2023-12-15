using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Server.Shared;
using System.Text.RegularExpressions;

namespace Lycoris.Blog.Server.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsoleMiddleware : BaseMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="factory"></param>
        public ConsoleMiddleware(RequestDelegate next, ILycorisLoggerFactory factory) : base(next, factory.CreateLogger<ConsoleMiddleware>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task InvokeHandlerAsync(HttpContext context)
        {
            if (context.Request.Method.ToLower() != "get" || !context.Request.Path.HasValue || IsStaticFileReuqest(context))
            {
                await _next.Invoke(context);
                return;
            }

            var path = context.Request.Path.Value.ToLower();

            if (path.StartsWith("/console"))
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (path.StartsWith("/zzyo/"))
                context.Request.Path = Regex.Replace(context.Request.Path, "^/zzyo/", "/console/"); ;

            await _next.Invoke(context);
        }
    }
}
