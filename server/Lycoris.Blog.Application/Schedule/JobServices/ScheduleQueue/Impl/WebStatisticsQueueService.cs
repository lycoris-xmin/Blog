using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Common;
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

            var hepler = new UserAgentInfo(userAgent!);

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


            config.OSStatistics ??= new List<CommonStatisticsConfiguration>();

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

            config.DeviceStatistics ??= new List<CommonStatisticsConfiguration>();

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
    }
}
