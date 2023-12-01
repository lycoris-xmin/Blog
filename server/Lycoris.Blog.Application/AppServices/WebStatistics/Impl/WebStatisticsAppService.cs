using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.WebStatistics.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.WebStatistics.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class WebStatisticsAppService : ApplicationBaseService, IWebStatisticsAppService
    {
        private readonly IRepository<BrowseStatistics, int> _browseStatistics;
        private readonly IRepository<RefererStatistics, int> _refererStatistics;
        private readonly IRepository<BrowseWorldMap, int> _browseWorldMap;

        public WebStatisticsAppService(IRepository<BrowseStatistics, int> browseStatistics, IRepository<RefererStatistics, int> refererStatistics, IRepository<BrowseWorldMap, int> browseWorldMap)
        {
            _browseStatistics = browseStatistics;
            _refererStatistics = refererStatistics;
            _browseWorldMap = browseWorldMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public async Task<PageResultDto<BrowseStatisticsDataDto>> GetBrowseStatisticsListAsync(int pageIndex, int pageSize, bool sum)
        {
            var filter = _browseStatistics.GetAll().Where(x => x.Count > 0);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<BrowseStatisticsDataDto>(count);

            var query = filter.OrderByDescending(x => x.Count)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new BrowseStatisticsDataDto()
                              {
                                  Route = x.Route,
                                  PageName = x.PageName,
                                  Count = x.Count
                              });

            var list = await query.ToListAsync();

            BrowseStatisticsDataDto? summary = null;
            if (sum)
            {
                summary = new BrowseStatisticsDataDto
                {
                    Count = await filter.SumAsync(x => x.Count)
                };
            }

            return new PageResultDto<BrowseStatisticsDataDto>(count, summary, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public async Task<PageResultDto<RefererStatisticsDataDto>> GetRefererStatisticsListAsync(int pageIndex, int pageSize, bool sum)
        {
            var filter = _refererStatistics.GetAll().Where(x => x.Count > 0);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<RefererStatisticsDataDto>(count);

            var query = filter.OrderByDescending(x => x.Count)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new RefererStatisticsDataDto()
                              {
                                  Referer = x.Referer,
                                  Domain = x.Domain,
                                  Count = x.Count
                              });

            var list = await query.ToListAsync();

            RefererStatisticsDataDto? summary = null;
            if (sum)
            {
                summary = new RefererStatisticsDataDto
                {
                    Count = await filter.SumAsync(x => x.Count)
                };
            }

            return new PageResultDto<RefererStatisticsDataDto>(count, summary, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<WorldBrowseMapDataDto>> GetWorldBrowseMapListAsync()
        {
            var qyert = _browseWorldMap.GetAll()
                                       .Where(x => x.Count > 0)
                                       .Select(x => new WorldBrowseMapDataDto()
                                       {
                                           Country = x.Country,
                                           Count = x.Count
                                       });

            var list = await qyert.ToListAsync();

            return list.OrderByDescending(x => x.Count).ToList();
        }
    }
}
