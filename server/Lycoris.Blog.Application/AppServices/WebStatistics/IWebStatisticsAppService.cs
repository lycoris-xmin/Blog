using Lycoris.Blog.Application.AppServices.WebStatistics.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.WebStatistics
{
    public interface IWebStatisticsAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        Task<PageResultDto<BrowseStatisticsDataDto>> GetBrowseStatisticsListAsync(int pageIndex, int pageSize, bool sum);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        Task<PageResultDto<RefererStatisticsDataDto>> GetRefererStatisticsListAsync(int pageIndex, int pageSize, bool sum);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<WorldBrowseMapDataDto>> GetWorldBrowseMapListAsync();
    }
}
