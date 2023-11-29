using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.WebStatistics.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.WebStatistics.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class WebStatisticsAppService : ApplicationBaseService, IWebStatisticsAppService
    {
        private readonly IRepository<BrowseWorldMap, int> _browseWorldMap;

        public WebStatisticsAppService(IRepository<BrowseWorldMap, int> browseWorldMap)
        {
            _browseWorldMap = browseWorldMap;
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
