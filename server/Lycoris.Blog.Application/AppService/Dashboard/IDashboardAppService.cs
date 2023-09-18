using Lycoris.Blog.Application.AppService.Dashboard.Dtos;
using Lycoris.Blog.Application.Shared;

namespace Lycoris.Blog.Application.AppService.Dashboard
{
    public interface IDashboardAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<StatisticsDto> GetWebStatisticsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<NearlyDaysWebStatisticsDataDto>> GetNearlyDaysWebStatisticsListAsync();
    }
}
