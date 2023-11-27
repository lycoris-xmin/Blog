using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.Authentication;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.FreezeUsers.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class FreezeUserAppService : ApplicationBaseService, IFreezeUserAppService
    {
        private readonly IRepository<FreezeUser, long> _repository;
        private readonly IAuthenticationCacheService _cache;

        public FreezeUserAppService(IRepository<FreezeUser, long> repository, IAuthenticationCacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<List<FreezeUser>> GetFreeUserListAsync() => _repository.GetAll().Where(x => x.FreeEndTime > DateTime.Now).ToListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="failedCount"></param>
        /// <returns></returns>
        public async Task SetFreeUserAsync(long id, int failedCount)
        {
            if (failedCount < 3)
                return;

            var data = await _repository.GetAsync(id) ?? new FreezeUser() { FreeBeginTime = DateTime.MinValue, FreeEndTime = DateTime.MaxValue };

            if (data.Id == 0 || data.FreeEndTime == DateTime.MaxValue || data.FreeEndTime < DateTime.Now)
            {
                data.FreeBeginTime = DateTime.Now;
                // 设置冻结时间
                // 
                data.FreeEndTime = DateTime.Now.AddMinutes(30);
            }

            if (data.Id == 0)
            {
                data.Id = id;
                await _repository.CreateAsync(data);
            }
            else
                await _repository.UpdateFieIdsAsync(data, x => x.FreeBeginTime, x => x.FreeEndTime);

            _cache.SetFreezeUser(id, data.FreeEndTime);
        }
    }
}
