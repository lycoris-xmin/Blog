using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.BrowseLogs.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.BrowseLogs.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class BrowseLogAppService : ApplicationBaseService, IBrowseLogAppService
    {
        private readonly IRepository<BrowseLog, long> _browseLog;

        public BrowseLogAppService(IRepository<BrowseLog, long> browseLog)
        {
            _browseLog = browseLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<BrowseLogDataDto>> GetListAsync(BrowseLogListFilter input)
        {
            var filter = _browseLog.GetAll()
                                   .WhereIf(input.BeginTime.HasValue, x => x.CreateTime >= input.BeginTime!.Value)
                                   .WhereIf(input.EndTime.HasValue, x => x.CreateTime <= input.EndTime!.Value)
                                   .WhereIf(!input.Route.IsNullOrEmpty(), x => x.Route == input.Route)
                                   .WhereIf(input.Ip.HasValue, x => x.Ip == input.Ip!.Value)
                                   .WhereIf(!input.Referer.IsNullOrEmpty(), x => x.Referer == input.Referer);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<BrowseLogDataDto>(count);

            var query = filter.OrderByDescending(x => x.CreateTime)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new BrowseLogDataDto()
                              {
                                  Id = x.Id,
                                  Route = x.Route,
                                  PageName = x.PageName,
                                  Ip = x.Ip,
                                  IpAddress = x.IpAddress,
                                  Referer = x.Referer,
                                  CreateTime = x.CreateTime
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<BrowseLogDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteAsync(params long[] ids)
        {
            if (!ids.HasValue())
                return;

            var sql = $"DELETE FROM {_browseLog.TableName} WHERE Id IN ({string.Join(",", ids)})";
            await _browseLog.ExecuteNonQueryAsync(sql);
        }
    }
}
