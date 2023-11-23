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

            config!.TotalBrowse += model.Browse;
            config!.TotalMessage += model.Message;
            config!.TotalUsers += model.User;

            seting!.Value = config.ToJson();

            await _configuration.SaveConfigurationAsync(seting);
        }
    }
}
