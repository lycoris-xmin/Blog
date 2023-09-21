using Lycoris.Blog.Application.AppService.Configurations;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Quartz.Extensions.Job;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("网站数据统计", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0 1 * * ?")]
    public class WebStatisticsJob : BaseJob
    {
        private readonly IRepository<WebDayStatistics, int> _webDayStatistics;
        private readonly IRepository<RequestLog, long> _requestLog;
        private readonly IRepository<BrowseLog, long> _browseLog;
        private readonly IRepository<LeaveMessage, int> _leaveMessage;
        private readonly IRepository<User, long> _user;
        private readonly IConfigurationAppService _configuration;

        public WebStatisticsJob(ILycorisLoggerFactory factory,
                                IRepository<WebDayStatistics, int> webDayStatistics,
                                IRepository<RequestLog, long> requestLog,
                                IRepository<BrowseLog, long> browseLog,
                                IRepository<LeaveMessage, int> leaveMessage,
                                IRepository<User, long> user,
                                IConfigurationAppService configuration) : base(factory.CreateLogger<WebStatisticsJob>())
        {
            _webDayStatistics = webDayStatistics;
            _requestLog = requestLog;
            _browseLog = browseLog;
            _configuration = configuration;
            _leaveMessage = leaveMessage;
            _user = user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync(IJobExecutionContext context)
        {
            var value = await _configuration.GetConfigurationAsync<WebStatisticsConfiguration>(AppConfig.WebStatistics) ?? new WebStatisticsConfiguration();
            if (!value.LastTime.HasValue || value.LastTime == DateTime.MinValue)
            {
                value.LastTime = await _requestLog.GetAll().OrderBy(x => x.CreateTime).Select(x => x.CreateTime).FirstOrDefaultAsync();
                value.LastTime ??= DateTime.Now;
                value.LastTime = value.LastTime.Value.Date;
            }

            var startDate = value.LastTime.Value;
            var endDate = startDate.AddDays(1);
            var absoultEndDate = DateTime.Now.Date;

            do
            {
                var toDayData = await _webDayStatistics.GetAll().Where(x => x.Day == startDate).SingleOrDefaultAsync() ?? new WebDayStatistics() { Day = startDate };

                toDayData.Api = await _requestLog.GetAll().Where(x => x.CreateTime >= startDate && x.CreateTime < endDate).CountAsync();
                toDayData.ErrorApi = await _requestLog.GetAll().Where(x => x.CreateTime >= startDate && x.CreateTime < endDate).Where(x => x.Success == false).CountAsync();
                toDayData.PVBrowse = await _browseLog.GetAll().Where(x => x.CreateTime >= startDate && x.CreateTime < endDate).CountAsync();
                toDayData.UVBrowse = await _browseLog.GetAll().Where(x => x.CreateTime >= startDate && x.CreateTime < endDate).GroupBy(x => x.ClientOrign).Select(x => 1).SumAsync(x => x);

                if (toDayData.Id > 0)
                    await _webDayStatistics.UpdateAsync(toDayData);
                else
                    await _webDayStatistics.CreateAsync(toDayData);

                startDate = startDate.AddDays(1);
                endDate = endDate.AddDays(1);

                // 每日浏览累加至网站全部统计
                value.TotalBrowse += toDayData.PVBrowse;

                // 休眠一秒
                if (endDate <= absoultEndDate)
                    Thread.CurrentThread.Join(1000);

            } while (endDate <= absoultEndDate);

            // 计算其他累计信息
            value.TotalMessage += await _leaveMessage.GetAll().Where(x => x.CreateTime >= value.LastTime.Value && x.CreateTime < absoultEndDate).CountAsync();
            value.TotalUsers += await _user.GetAll().Where(x => x.CreateTime >= value.LastTime.Value && x.CreateTime < absoultEndDate).CountAsync();

            // 赋值当前统计时间
            value.LastTime = absoultEndDate;

            // 更新
            await _configuration.SaveConfigurationAsync(AppConfig.WebStatistics, value);
        }
    }
}
