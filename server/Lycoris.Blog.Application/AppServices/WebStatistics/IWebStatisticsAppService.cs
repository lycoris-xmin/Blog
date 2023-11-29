using Lycoris.Blog.Application.AppServices.WebStatistics.Dtos;
using Lycoris.Blog.Application.Shared;

namespace Lycoris.Blog.Application.AppServices.WebStatistics
{
    public interface IWebStatisticsAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<WorldBrowseMapDataDto>> GetWorldBrowseMapListAsync();
    }
}
