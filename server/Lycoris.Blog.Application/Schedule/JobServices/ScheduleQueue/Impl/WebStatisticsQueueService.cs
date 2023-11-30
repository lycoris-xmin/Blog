using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "WebStatistics")]
    public class WebStatisticsQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }
        public JobLogger? JobLogger { get; set; }

        private readonly IConfigurationRepository _configuration;

        public WebStatisticsQueueService(IConfigurationRepository configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var model = data.ToObject<WebStatisticsQueueModel>();
            if (model == null)
            {
                this.JobLogger!.Error("can not find any data");
                return;
            }

            var seting = await _configuration.GetDataAsync(AppConfig.WebStatistics);

            var config = seting!.Value.ToObject<WebStatisticsConfiguration>();

            if (config!.TotalBrowse < int.MaxValue)
                config!.TotalBrowse += model.Browse;

            if (config!.TotalMessage < int.MaxValue)
                config!.TotalMessage += model.Message;

            if (config!.TotalUsers < int.MaxValue)
                config!.TotalUsers += model.User;

            HandleUserAgentStatistics(config!, model.UserAgent);

            seting!.Value = config.ToJson();

            await _configuration.SaveConfigurationAsync(seting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="userAgent"></param>
        private static void HandleUserAgentStatistics(WebStatisticsConfiguration config, string? userAgent)
        {
            if (userAgent.IsNullOrEmpty())
                return;

            config.BrowserStatistics ??= new List<CommonStatisticsConfiguration>();

            var hepler = new UserAgentHelper(userAgent!);

            var browser = config.BrowserStatistics.SingleOrDefault(x => x.Name == hepler.Browser);
            if (browser != null)
            {
                browser.Count++;
                return;
            }

            config.BrowserStatistics.Add(new CommonStatisticsConfiguration()
            {
                Name = hepler.Browser,
                Count = 1,
                Icon = hepler.BrowserIcon
            });


            config.OSStatistics ??= new List<CommonStatisticsConfiguration>();

            var os = config.OSStatistics.SingleOrDefault(x => x.Name == hepler.OS);
            if (os != null)
            {
                os.Count++;
                return;
            }

            config.OSStatistics.Add(new CommonStatisticsConfiguration()
            {
                Name = hepler.OS,
                Count = 1,
                Icon = hepler.OSIcon
            });


            config.DeviceStatistics ??= new List<CommonStatisticsConfiguration>();

            var device = config.DeviceStatistics.SingleOrDefault(x => x.Name == hepler.OS);
            if (device != null)
            {
                device.Count++;
                return;
            }

            config.DeviceStatistics.Add(new CommonStatisticsConfiguration()
            {
                Name = hepler.Device,
                Count = 1,
                Icon = hepler.DeviceIcon
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private class UserAgentHelper
        {
            public string Browser { get; set; } = string.Empty;

            public string BrowserIcon { get; set; } = string.Empty;

            public string OS { get; set; } = string.Empty;

            public string OSIcon { get; set; } = string.Empty;

            public string Device { get; set; } = string.Empty;

            public string DeviceIcon { get; set; } = string.Empty;

            public UserAgentHelper(string userAgent)
            {
                BrowserInit(userAgent);

                OSInit(userAgent);

                DeviceInit(userAgent);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userAgent"></param>
            private void BrowserInit(string userAgent)
            {
                // 判断常见的浏览器
                if (userAgent!.Contains("MSIE") || userAgent!.Contains("Trident"))
                {
                    // 用户正在使用 Internet Explorer
                    this.Browser = "Internet Explorer";
                    this.BrowserIcon = "";
                }
                else if (userAgent!.Contains("Edge"))
                {
                    // 用户正在使用 Microsoft Edge
                    this.Browser = "Microsoft Edge";
                    this.BrowserIcon = "";
                }
                else if (userAgent!.Contains("Chrome"))
                {
                    // 用户正在使用 Google Chrome
                    this.Browser = "Chrome";
                    this.BrowserIcon = "";
                }
                else if (userAgent!.Contains("Firefox"))
                {
                    // 用户正在使用 Mozilla Firefox
                    this.Browser = "Firefox";
                    this.BrowserIcon = "";
                }
                else if (userAgent!.Contains("Safari") && !userAgent!.Contains("Chrome"))
                {
                    // 用户正在使用 Apple Safari
                    this.Browser = "Safari";
                    this.BrowserIcon = "";
                }
                else
                {
                    // 未知浏览器
                    this.Browser = "未知";
                    this.BrowserIcon = "";
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userAgent"></param>
            private void OSInit(string userAgent)
            {
                if (userAgent.Contains("Windows"))
                {
                    this.OS = "Windows";
                    this.OSIcon = "";
                }
                else if (userAgent.Contains("Mac"))
                {
                    this.OS = "Mac OS";
                    this.OSIcon = "";
                }
                else if (userAgent.Contains("Android"))
                {
                    this.OS = "Android";
                    this.OSIcon = "";
                }
                else if (userAgent.Contains("iOS"))
                {
                    this.OS = "IOS";
                    this.OSIcon = "";
                }
                else
                {
                    this.OS = "未知";
                    this.OSIcon = "";
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userAgent"></param>
            private void DeviceInit(string userAgent)
            {
                if (userAgent.Contains("Mobile"))
                {
                    this.Device = "手机";
                    this.DeviceIcon = "";
                }
                else if (userAgent.Contains("Tablet"))
                {
                    this.Device = "笔记本";
                    this.DeviceIcon = "";
                }
                else
                {
                    this.Device = "台式机";
                    this.DeviceIcon = "";
                }
            }
        }
    }
}
