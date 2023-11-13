using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.AccessControls.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.AccessControls.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true, EnableInterceptor = true)]
    public class AccessControlAppService : ApplicationBaseService, IAccessControlAppService
    {
        private readonly IRepository<AccessControl, int> _accessControl;
        private readonly IRepository<AccessControlLog, long> _accessControlLog;

        public AccessControlAppService(IRepository<AccessControl, int> accessControl, IRepository<AccessControlLog, long> accessControlLog)
        {
            _accessControl = accessControl;
            _accessControlLog = accessControlLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<AccessControlDataDto>> GetListAsync(GetAccessControlListFilter input)
        {
            uint? ip = input.Ip.IsNullOrEmpty() ? null : IPAddressHelper.Ipv4ToUInt32(input.Ip!);

            var filter = _accessControl.GetAll().WhereIf(ip.HasValue, x => x.Ip == ip!.Value);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<AccessControlDataDto>(count);

            var query = filter.OrderByDescending(x => x.LastAccessTime)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new AccessControlDataDto()
                              {
                                  Id = x.Id,
                                  Ip = x.Ip,
                                  IpAddress = x.IpAddress,
                                  Count = x.Count,
                                  LastAccessTime = x.LastAccessTime
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<AccessControlDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<AccessControlDataDto> CreateAsync(string ip)
        {
            var data = new AccessControl()
            {
                Ip = IPAddressHelper.Ipv4ToUInt32(ip),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(ip)),
                Count = 0,
                LastAccessTime = DateTime.Now
            };

            var repeat = await _accessControl.GetAll().Where(x => x.Ip == data.Ip).AnyAsync();
            if (repeat)
                throw new FriendlyException($"IP：{ip} 已存在访问管控列表中");

            data = await _accessControl.CreateAsync(data);

            return data.ToMap<AccessControlDataDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transactional]
        public async Task DeleteAsync(int id)
        {
            var data = await _accessControl.GetAsync(id);
            if (data == null)
                return;

            await _accessControl.DeleteAsync(id);

            await _accessControlLog.ExecuteNonQueryAsync($"DELETE FROM {_accessControlLog.TableName} WHERE {nameof(AccessControlLog.AccessControlId)} = {id}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<AccessControlLogDataDto>> GetAccessControlLogListAsync(GetAccessControlLogListFilter input)
        {
            var filter = _accessControlLog.GetAll().Where(x => x.AccessControlId == input.AccessControlId);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<AccessControlLogDataDto>(count);

            var query = filter.OrderByDescending(x => x.CreateTime)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new AccessControlLogDataDto()
                              {
                                  Id = x.Id,
                                  Method = x.Method,
                                  Route = x.Route,
                                  Params = x.Params,
                                  StatusCode = x.StatusCode,
                                  Response = x.Response,
                                  CreateTime = x.CreateTime,
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<AccessControlLogDataDto>(count, list);
        }
    }
}
