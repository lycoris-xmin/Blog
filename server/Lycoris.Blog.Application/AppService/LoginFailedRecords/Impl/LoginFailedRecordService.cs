using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppService.LoginFailedRecords.Dtos;
using Lycoris.Blog.Application.Cached.LoginFailedRecordCache;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;

namespace Lycoris.Blog.Application.AppService.LoginFailedRecords.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class LoginFailedRecordService : ILoginFailedRecordService
    {
        private readonly IRepository<LoginFailedRecord, int> _repository;
        private readonly ILoginFailedRecordCacheService _cache;

        public LoginFailedRecordService(IRepository<LoginFailedRecord, int> repository, ILoginFailedRecordCacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IQueryable<LoginFailedRecord> GetAll() => _repository.GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<LoginFailedRecordDto> GetLoginFailedRecordAsync(string email)
        {
            var res = new LoginFailedRecordDto();

            var cache = await _cache.GetLoginFailedRecordAsync(email);
            if (cache != null)
            {
                res.Count = cache.Count;
                res.FreezeTime = cache.FreezeTime;
            }
            else
            {
                var data = await _repository.GetAsync(x => x.Email == email);
                res.Count = data?.Count ?? 0;
                res.FreezeTime = data?.FreezeTime;
            }

            if (res.FreezeTime < DateTime.Now)
            {
                res.Count = 0;
                res.FreezeTime = null;
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task SetLoginFailedRecordAsync(LoginFailedRecord record)
        {
            // 写入数据库
            if (record.Id == 0)
                await _repository.CreateAsync(record);
            else
                await _repository.UpdateAsync(record);

            // 写入缓存
            await _cache.SetLoginFailedRecordAsync(record.Email, new LoginFailedRecordDto(record.Count, record.FreezeTime));
        }
    }
}
