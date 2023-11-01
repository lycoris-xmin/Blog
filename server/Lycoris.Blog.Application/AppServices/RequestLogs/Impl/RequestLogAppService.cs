using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.RequestLogs.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.RequestLogs.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class RequestLogAppService : ApplicationBaseService, IRequestLogAppService
    {
        private readonly IRepository<RequestLog, long> _requestLog;

        public RequestLogAppService(IRepository<RequestLog, long> requestLog)
        {
            _requestLog = requestLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<RequestLogDataDto>> GetListAsync(GetRequestLogListFilter input)
        {
            var filter = _requestLog.GetAll()
                                    .WhereIf(input.BeginTime.HasValue, x => x.CreateTime >= input.BeginTime)
                                    .WhereIf(input.EndTime.HasValue, x => x.CreateTime <= input.EndTime)
                                    .WhereIf(input.Ip.HasValue && input.Ip > 0, x => x.IP == input.Ip!.Value)
                                    .WhereIf(!input.Route.IsNullOrEmpty(), x => EF.Functions.Like(x.Route, $"%{input.Route!}%"))
                                    .WhereIf(input.Elapsed.HasValue && input.Elapsed.Value == 1, x => x.ElapsedMilliseconds <= 2000)
                                    .WhereIf(input.Elapsed.HasValue && input.Elapsed.Value == 2, x => x.ElapsedMilliseconds > 2000 && x.ElapsedMilliseconds <= 5000)
                                    .WhereIf(input.Elapsed.HasValue && input.Elapsed.Value == 3, x => x.ElapsedMilliseconds > 5000 && x.ElapsedMilliseconds <= 10000)
                                    .WhereIf(input.Elapsed.HasValue && input.Elapsed.Value == 4, x => x.ElapsedMilliseconds > 10000)
                                    .WhereIf(input.Status.HasValue && input.Status.Value == 1, x => x.StatusCode >= 200 && x.StatusCode < 300)
                                    .WhereIf(input.Status.HasValue && input.Status.Value == 2, x => x.StatusCode >= 300)
                                    .WhereIf(input.Success.HasValue, x => x.Success == input.Success!.Value);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<RequestLogDataDto>(count);

            var query = filter.OrderByDescending(x => x.CreateTime)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new RequestLogDataDto()
                              {
                                  Id = x.Id,
                                  Route = x.Route,
                                  Success = x.Success,
                                  ElapsedMilliseconds = x.ElapsedMilliseconds,
                                  IP = x.IP,
                                  IPAddress = x.IPAddress,
                                  CreateTime = x.CreateTime
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<RequestLogDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RequestLogInfoDto> GetInfoAsync(long id)
        {
            var data = await _requestLog.GetAsync(id) ?? throw new FriendlyException("数据不存在或已被删除");

            return data.ToMap<RequestLogInfoDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteAsync(params long[] ids)
        {
            if (ids.HasValue())
            {
                var sql = $"DELETE FROM {_requestLog.TableName} WHERE Id IN ({string.Join(",", ids)});";
                await _requestLog.ExecuteNonQueryAsync(sql);
            }
        }
    }
}
