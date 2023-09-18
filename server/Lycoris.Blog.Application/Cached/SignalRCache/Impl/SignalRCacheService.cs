using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.SignalR.Shared.Dtos;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Cache;
using Lycoris.CSRedisCore.Extensions;

namespace Lycoris.Blog.Application.Cached.SignalRCache.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class SignalRCacheService : ISignalRCacheService
    {
        private readonly Lazy<IMemoryCacheManager> _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        public SignalRCacheService(Lazy<IMemoryCacheManager> memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task<SignalRConnectionDto?> GetSignalRConnectionAsync(string connectionId)
        {
            if (AppSettings.Redis.Use)
                return await RedisCache.String.GetAsync<SignalRConnectionDto>(GetConnectionCacheKey(connectionId));
            else
                return _memoryCache.Value.GetMemory<SignalRConnectionDto>(GetConnectionCacheKey(connectionId));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task SetSignalRConnectionAsync(string connectionId, SignalRConnectionDto value, TimeSpan time)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.String.SetAsync(GetConnectionCacheKey(connectionId), value, time);
            else
                _memoryCache.Value.CreateMemory(GetConnectionCacheKey(connectionId), value, DateTime.Now.AddSeconds(time.TotalSeconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task RemoveSignalRConnectionAsync(string connectionId)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.Key.RemoveAsync(GetConnectionCacheKey(connectionId));
            else
                _memoryCache.Value.RemoveMemory(GetConnectionCacheKey(connectionId));
        }

        private static string GetConnectionCacheKey(string connectionId) => $"SignalRConnection:{connectionId}";
    }
}
