using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Contexts;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Lycoris.Common.Utils.SensitiveWord;
using Lycoris.Quartz.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace Lycoris.Blog.Server.Application
{
    /// <summary>
    /// 程序启动任务
    /// </summary>
    public class ApplicationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestRouteMap _routeMap;
        private readonly AppMonitorContext _serverMonitor;
        private readonly IQuartzSchedulerCenter _scheduler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ApplicationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _routeMap = serviceProvider.GetRequiredService<RequestRouteMap>();
            _serverMonitor = serviceProvider.GetRequiredService<AppMonitorContext>();
            _scheduler = serviceProvider.GetRequiredService<IQuartzSchedulerCenter>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            RequestRouteMapInit();

            _serverMonitor.BeginRunTime = DateTime.Now;

            using var scope = _serviceProvider.CreateScope();

            // 数据库迁移，预热
            await EntityFrameworkCoreWarmUpAsync(scope.ServiceProvider);

            await SensitiveWordStoreInitAsync();

            await _scheduler.ManualRunNonStandbyJobsAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// 数据库迁移，预热
        /// </summary>
        /// <param name="provider"></param>
        private static Task EntityFrameworkCoreWarmUpAsync(IServiceProvider provider)
        {
            var context = provider.GetRequiredService<MySqlContext>();
            return context.WarmUpAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void RequestRouteMapInit()
        {
            var assembly = GetType().Assembly;
            //获取所有可用controller
            var controllers = assembly.GetTypes().Where(x => x.BaseType == typeof(BaseController)).ToList();

            var actionType = new Type[2] { typeof(HttpGetAttribute), typeof(HttpPostAttribute) };

            foreach (var controller in controllers)
            {
                //获取所有包含http特性且不包含过时特性的接口方法
                var methods = controller.GetMethods().Where(x => x.IsPublic && x.CustomAttributes.Any(x => actionType.Contains(x.AttributeType))).ToList() ?? new List<MethodInfo>();

                var controllerRoute = controller.GetCustomAttribute<RouteAttribute>() ?? new RouteAttribute(controller.Name.Replace("Controller", ""));

                var isDashboradApi = controller.GetCustomAttribute<AppAuthenticationAttribute>() != null;

                foreach (var method in methods)
                {
                    if (!isDashboradApi)
                        isDashboradApi = method.GetCustomAttribute<AppAuthenticationAttribute>() != null;

                    if (method.GetCustomAttributes().Where(x => x is HttpMethodAttribute).SingleOrDefault() is HttpMethodAttribute httpMethod)
                    {
                        if (!isDashboradApi)
                            isDashboradApi = httpMethod.Template!.StartsWith("Dashboard");

                        var route = $"/{controllerRoute.Template}/{httpMethod.Template}";
                        if (isDashboradApi)
                            _routeMap.DashboardRoute.Add(route);
                        else
                            _routeMap.WebRoute.Add(route);
                    }
                }
            }

            if (_routeMap.DashboardRoute.HasValue() || _routeMap.WebRoute.HasValue())
            {
                // 获取日志记录配置
                // 仅博客网站进行请求日志记录
                _routeMap.LogFilter = RequestLogFilterEnum.OnlyWeb;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task SensitiveWordStoreInitAsync()
        {
            var util = await new SensitiveWordUtils().LoadTxtFileAsync(AppSettings.Path.SensitiveWord);
            util.AsMempryStore();
        }
    }
}
