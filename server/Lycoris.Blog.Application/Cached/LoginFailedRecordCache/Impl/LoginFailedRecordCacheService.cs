using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppService.LoginFailedRecords.Dtos;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Cache;
using Lycoris.CSRedisCore.Extensions;

namespace Lycoris.Blog.Application.Cached.LoginFailedRecordCache.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class LoginFailedRecordCacheService : ILoginFailedRecordCacheService
    {
        private readonly Lazy<IMemoryCacheManager> _memoryCache;

        public LoginFailedRecordCacheService(Lazy<IMemoryCacheManager> memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public async Task<LoginFailedRecordDto?> GetLoginFailedRecordAsync(string email)
        {
            if (AppSettings.Redis.Use)
                return await RedisCache.String.GetAsync<LoginFailedRecordDto>(GetCacheKey(email));
            else
                return _memoryCache.Value.TryGetMemory<LoginFailedRecordDto>(GetCacheKey(email));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="data"></param>
        public async Task SetLoginFailedRecordAsync(string email, LoginFailedRecordDto data)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetCacheKey(email), data, TimeSpan.FromMinutes(15));
            else
                _memoryCache.Value.CreateMemory(GetCacheKey(email), data, DateTime.Now.AddMinutes(15));
        }

        private static string GetCacheKey(string email) => $"LoginFailed:{email}";
    }
}
