using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.BrowseLogs.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppService.BrowseLogs.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class BrowseLogAppService : ApplicationBaseService, IBrowseLogAppService
    {
        private readonly IRepository<BrowseLog, long> _browseLog;
        private readonly IRepository<BrowseReferer, int> _browseReferer;

        public BrowseLogAppService(IRepository<BrowseLog, long> browseLog, IRepository<BrowseReferer, int> browseReferer)
        {
            _browseLog = browseLog;
            _browseReferer = browseReferer;
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
                                   .WhereIf(!input.Path.IsNullOrEmpty(), x => x.Path == input.Path)
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
                                  Path = x.Path,
                                  PageName = x.PageName,
                                  UserAgent = x.UserAgent,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResultDto<BrowseRefererDataDto>> GetRefererListAsync(int pageIndex, int pageSize)
        {
            var filter = _browseReferer.GetAll();
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<BrowseRefererDataDto>(count);

            var query = filter.OrderByDescending(x => x.Count)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new BrowseRefererDataDto()
                              {
                                  Id = x.Id,
                                  Domain = x.Domain,
                                  Referer = x.Referer,
                                  Count = x.Count
                              });

            var list = await query.ToListAsync();
            return new PageResultDto<BrowseRefererDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteRefererAsync(params long[] ids)
        {
            if (!ids.HasValue())
                return;

            var sql = $"DELETE FROM {_browseReferer.TableName} WHERE Id IN ({string.Join(",", ids)})";
            await _browseReferer.ExecuteNonQueryAsync(sql);
        }
    }
}
