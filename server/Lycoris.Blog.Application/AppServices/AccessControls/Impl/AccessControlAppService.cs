using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.AccessControls.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
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
        [Transactional]
        public async Task<PageResultDto<AccessControlDataDto>> GetListAsync(GetAccessControlListFilter input)
        {
            uint? ip = input.Ip.IsNullOrEmpty() ? null : IPAddressHelper.Ipv4ToUInt32(input.Ip!);

            var filter = _accessControl.GetAll().WhereIf(ip.HasValue, x => x.Ip == ip!.Value);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<AccessControlDataDto>(count);

            var query = filter.PageBy(input.PageIndex, input.PageSize)
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
        /// <returns></returns>
        public async Task<PageResultDto<AccessControlDataDto>> GetListAsync()
        {
            return new PageResultDto<AccessControlDataDto>();
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
    }
}
