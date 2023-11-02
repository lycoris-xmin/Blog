using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.LoginFailedRecord.Models;
using Lycoris.Blog.Common.Cache;

namespace Lycoris.Blog.Application.Cached.LoginFailedRecord.Impl
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
        public LoginFailedRecordCacheModel? GetLoginFailedRecord(string email) => _memoryCache.Value.TryGetMemory<LoginFailedRecordCacheModel>(GetCacheKey(email));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="data"></param>
        public void SetLoginFailedRecord(string email, LoginFailedRecordCacheModel data) => _memoryCache.Value.CreateMemory(GetCacheKey(email), data, DateTime.Now.AddMinutes(15));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static string GetCacheKey(string email) => $"LoginFailed:{email}";
    }
}
