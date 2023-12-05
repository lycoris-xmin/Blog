using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Quartz;
using System.Text.RegularExpressions;

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

            if (config!.TotalCommentMessage < int.MaxValue)
                config!.TotalCommentMessage += model.CommentMessage;

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
            }
            else
            {
                config.BrowserStatistics.Add(new CommonStatisticsConfiguration()
                {
                    Name = hepler.Browser,
                    Count = 1,
                    Icon = hepler.BrowserIcon
                });
            }


            config.OSStatistics = new List<CommonStatisticsConfiguration>();

            var os = config.OSStatistics.SingleOrDefault(x => x.Name == hepler.OS);
            if (os != null)
            {
                os.Count++;
            }
            else
            {
                config.OSStatistics.Add(new CommonStatisticsConfiguration()
                {
                    Name = hepler.OS,
                    Count = 1,
                    Icon = hepler.OSIcon
                });
            }

            config.DeviceStatistics = new List<CommonStatisticsConfiguration>();

            var device = config.DeviceStatistics.SingleOrDefault(x => x.Name == hepler.Device);
            if (device != null)
            {
                device.Count++;
            }
            else
            {
                config.DeviceStatistics.Add(new CommonStatisticsConfiguration()
                {
                    Name = hepler.Device,
                    Count = 1,
                    Icon = hepler.DeviceIcon
                });
            }
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
                foreach (var item in UaRegex)
                {
                    var match = Regex.Match(userAgent, item.Value);
                    if (match.Success)
                    {
                        this.Browser = item.Client;
                        this.BrowserIcon = item.ClientIcon;
                        break;
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userAgent"></param>
            private void OSInit(string userAgent)
            {
                // 判断操作系统
                if (Regex.IsMatch(userAgent, @"Windows NT 11.0"))
                {
                    this.OS = "Windows 11";
                    this.OSIcon = "windows.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT 10.0"))
                {
                    this.OS = "Windows 10";
                    this.OSIcon = "windows.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT 6.2"))
                {
                    this.OS = "Windows 8";
                    this.OSIcon = "windows-8.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT 6.1"))
                {
                    this.OS = "Windows 7";
                    this.OSIcon = "windows-7.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT 6.0"))
                {
                    this.OS = "Windows Vista";
                    this.OSIcon = "windows-7.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT 5.1"))
                {
                    this.OS = "Windows XP";
                    this.OSIcon = "windows-7.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT 5.0"))
                {
                    this.OS = "Windows 2000";
                    this.OSIcon = "windows-2000.png";
                }
                else if (Regex.IsMatch(userAgent, @"Mac OS X"))
                {
                    this.OS = "Mac OS";
                    this.OSIcon = "mac-os.png";
                }
                else if (Regex.IsMatch(userAgent, @"Linux"))
                {
                    this.OS = "Linux";
                    this.OSIcon = "linux.png";
                }
                else if (Regex.IsMatch(userAgent, @"Android"))
                {
                    this.OS = "Android";
                    this.OSIcon = "android.png";
                }
                else if (Regex.IsMatch(userAgent, @"iOS"))
                {
                    this.OS = "IOS";
                    this.OSIcon = "ios.png";
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
                // 判断设备类型
                if (Regex.IsMatch(userAgent, @"Windows NT") && Regex.IsMatch(userAgent, @"Win64; x64"))
                {
                    this.Device = "台式机";
                    this.DeviceIcon = "desktop.png";
                }
                else if (Regex.IsMatch(userAgent, @"Windows NT"))
                {
                    this.Device = "笔记本";
                    this.DeviceIcon = "laptop.png";
                }
                else if (Regex.IsMatch(userAgent, @"Mobile"))
                {
                    this.Device = "手机";
                    this.DeviceIcon = "mobile.png";
                }
                else if (Regex.IsMatch(userAgent, @"iPad|Android|tablet"))
                {
                    this.Device = "平板";
                    this.DeviceIcon = "tablet.png";
                }
                else
                {
                    this.Device = "未知设备";
                    this.DeviceIcon = "";
                }
            }

            /// <summary>
            /// 
            /// </summary>
            private readonly List<UserAgentInfo> UaRegex = new()
            {
                new UserAgentInfo("猎豹浏览器", "liebao.png", "LBBROWSER"),
                new UserAgentInfo("QQ浏览器", "qq.png", " QQBrowser"),
                new UserAgentInfo("百度浏览器", "baidu.png", "BIDUBrowser|baidubrowser|BaiduHD"),
                new UserAgentInfo("UC浏览器", "uc.png", "UBrowser|UCBrowser|UCWEB"),
                new UserAgentInfo("小米浏览器", "android.png", "MiuiBrowser"),
                new UserAgentInfo("微信", "android.png", "MicroMessenger"),
                new UserAgentInfo("手机QQ", "qq.png", "Mobile/\\w{5,}\\sQQ/(\\d+[.\\d]+)"),
                new UserAgentInfo("手机百度", "baidu.png", "baiduboxapp"),
                new UserAgentInfo("火狐浏览器", "firefox.png", "Firefox"),
                new UserAgentInfo("360安全浏览器", "360.png", "360SE"),
                new UserAgentInfo("360极速浏览器", "360.png", "360EE"),
                new UserAgentInfo("Opera", "opera.png", "Opera|OPR/(\\d+[.\\d]+)"),
                new UserAgentInfo("Microsoft Edge", "edge.png", "Edg"),
                new UserAgentInfo("安卓浏览器", "android.png", "Android.*Mobile\\sSafari|Android/(\\d[.\\d]+)\\sRelease/(\\d[.\\d]+)\\sBrowser/AppleWebKit(\\d[.\\d]+)"),
                new UserAgentInfo("IE浏览器", "ie.png", "Trident|MSIE"),
                new UserAgentInfo("Chrome", "chrome.png", "Chrome|CriOS"),
                new UserAgentInfo("Safari", "safari.png", "Version[|/](\\w.+)(\\s\\w.+)?\\s?Safari|like\\sGecko\\)\\sMobile/\\w{3,}$")
            };

            private class UserAgentInfo
            {
                public string Client { get; }
                public string ClientIcon { get; }
                public string Value { get; }

                public UserAgentInfo(string client, string clientIcon, string value)
                {
                    Client = client;
                    ClientIcon = clientIcon;
                    Value = value;
                }
            }
        }
    }
}
