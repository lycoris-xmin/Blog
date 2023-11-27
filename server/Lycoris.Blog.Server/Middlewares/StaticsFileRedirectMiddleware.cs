using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.Model.Cnstants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;

namespace Lycoris.Blog.Server.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticsFileRedirectMiddleware : BaseMiddleware
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="factory"></param>
        /// <param name="serviceProvider"></param>
        public StaticsFileRedirectMiddleware(RequestDelegate next, ILycorisLoggerFactory factory, IServiceProvider serviceProvider) : base(next, factory.CreateLogger<StaticsFileRedirectMiddleware>())
        {
            //this.IgnoreOpptionsReuqest = true;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task InvokeHandlerAsync(HttpContext context)
        {
            if (!IsStaticFileReuqest(context))
            {
                await _next.Invoke(context);
                return;
            }

            if (context.Request.Cookies.ContainsKey(HttpCookies.LocalStaticsFile))
            {
                var value = context.Request.Cookies[HttpCookies.LocalStaticsFile]!.ToString();
                if (!value.IsNullOrEmpty() && value == "1")
                {
                    await _next.Invoke(context);
                    return;
                }
            }

            var config = await GetConfigurationAsync();
            if (config == null || config.UploadChannel == FileUploadChannelEnum.Local || config.LoadFileSrc == LoadFileSrcEnum.Local)
            {
                await _next.Invoke(context);
                return;
            }

            var redirectUrl = "";

            if (config.UploadChannel == FileUploadChannelEnum.Github)
                redirectUrl = config.Github.ChangeJsDelivrCDNUrl(context.Request.Path.Value);
            else if (config.UploadChannel == FileUploadChannelEnum.Minio)
                redirectUrl = config.Minio.ChangeMonioFileUrl(context.Request.Path.Value);

            if (redirectUrl.IsNullOrEmpty())
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.Redirect(redirectUrl);
            _logger.Warn($"static file request path '{context.Request.Path.Value}' redirect to '{redirectUrl}'");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<UploadConfiguration?> GetConfigurationAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IConfigurationRepository>();
            return await repository.GetConfigurationAsync<UploadConfiguration>(AppConfig.Upload);
        }
    }
}
