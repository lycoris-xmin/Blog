using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.LoginRecords.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.LoginRecords.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class LoginRecordAppService : ApplicationBaseService, ILoginRecordAppService
    {
        private readonly IRepository<LoginRecord, int> _LoginRecord;

        public LoginRecordAppService(IRepository<LoginRecord, int> loginRecord)
        {
            _LoginRecord = loginRecord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResultDto<LoginRecordDataDto>> GetListAsync(int pageIndex, int pageSize)
        {
            var filter = _LoginRecord.GetAll().Where(x => x.UserId == CurrentUser.Id);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<LoginRecordDataDto>(count);

            var query = filter.OrderByDescending(x => x.LoginTime)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new LoginRecordDataDto()
                              {
                                  UserAgent = x.UserAgent,
                                  Ip = x.Ip,
                                  IpAddress = x.IpAddress,
                                  LoginTime = x.LoginTime,
                                  Success = x.Success,
                                  Remark = x.Remark
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<LoginRecordDataDto>(count, list);
        }
    }
}
