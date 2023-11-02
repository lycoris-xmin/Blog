using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.SignalR.Shared.Models;
using Lycoris.Blog.Common.Cache;

namespace Lycoris.Blog.Application.Cached.SignalR.Impl
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
        public SignalRConnectionModel? GetSignalRConnection(string connectionId) => _memoryCache.Value.GetMemory<SignalRConnectionModel>(GetConnectionCacheKey(connectionId));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void SetSignalRConnection(string connectionId, SignalRConnectionModel value, TimeSpan time) => _memoryCache.Value.CreateMemory(GetConnectionCacheKey(connectionId), value, DateTime.Now.AddSeconds(time.TotalSeconds));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public void RemoveSignalRConnection(string connectionId) => _memoryCache.Value.RemoveMemory(GetConnectionCacheKey(connectionId));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        private static string GetConnectionCacheKey(string connectionId) => $"SignalRConnection:{connectionId}";
    }
}
