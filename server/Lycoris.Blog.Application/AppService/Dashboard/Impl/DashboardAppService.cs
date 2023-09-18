using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.Dashboard.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppService.Dashboard.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class DashboardAppService : ApplicationBaseService, IDashboardAppService
    {
        private readonly IRepository<WebDayStatistics, int> _webStatistics;
        private readonly Lazy<IRepository<RequestLog, long>> _requestLog;
        private readonly Lazy<IRepository<BrowseLog, long>> _browseLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webStatistics"></param>
        /// <param name="requestLog"></param>
        /// <param name="browseLog"></param>
        public DashboardAppService(IRepository<WebDayStatistics, int> webStatistics, Lazy<IRepository<RequestLog, long>> requestLog, Lazy<IRepository<BrowseLog, long>> browseLog)
        {
            _webStatistics = webStatistics;
            _requestLog = requestLog;
            _browseLog = browseLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<StatisticsDto> GetWebStatisticsAsync()
        {
            var today = DateTime.Today;
            var dto = new StatisticsDto()
            {
                Browse = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync(),
                Api = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync(),
                ErrorApi = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= today).Where(x => x.Success == false).CountAsync()
            };

            var value = await this.ApplicationConfiguration.Value.GetConfigurationAsync<WebStatisticsConfiguration>(AppConfig.WebStatistics);

            dto.TotalBrowse = value?.TotalBrowse ?? 0;
            dto.TotalMessage = value?.TotalMessage ?? 0;
            dto.TotalUsers = value?.TotalUsers ?? 0;

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<NearlyDaysWebStatisticsDataDto>> GetNearlyDaysWebStatisticsListAsync()
        {
            var startTime = DateTime.Now.Date.Yesterday().AddDays(-6);
            var dayArray = new DateTime[7];
            for (int i = 0; i < 7; i++)
                dayArray[i] = startTime.AddDays(i);

            var filter = _webStatistics.GetAll().Where(x => x.Day >= dayArray[0] && x.Day <= dayArray[6]);

            var query = filter.Select(x => new NearlyDaysWebStatisticsDataDto()
            {
                Day = x.Day,
                PVBrowse = x.PVBrowse,
                UVBrowse = x.UVBrowse,
                Api = x.Api,
                ErrorApi = x.ErrorApi
            });

            var list = await query.ToListAsync();

            var days = dayArray.Except(list.Select(x => x.Day)).ToList();

            foreach (var item in days)
                list.Add(new NearlyDaysWebStatisticsDataDto(item));

            return list.OrderBy(x => x.Day).ToList();
        }
    }
}
