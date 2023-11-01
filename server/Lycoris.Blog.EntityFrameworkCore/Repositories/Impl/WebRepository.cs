using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common.Cache;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;

namespace Lycoris.Blog.EntityFrameworkCore.Repositories.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class WebSiteAboutRepository : IWebSiteAboutRepository
    {
        private readonly IRepository<WebSiteAbout, string> _repository;
        private readonly IMemoryCacheManager _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="memoryCache"></param>
        public WebSiteAboutRepository(IRepository<WebSiteAbout, string> repository, IMemoryCacheManager memoryCache)
        {
            _repository = repository;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<WebSiteAbout?> GetDataAsync(string configId) => _repository.GetAsync(x => x.Id == configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<string> GetAboutAsync(string configId)
        {
            var cache = GetCache(configId);
            if (!cache.IsNullOrEmpty())
                return cache!;

            cache = await _repository.GetSelectAsync(x => x.Id == configId, x => x.Value) ?? "";

            if (!cache.IsNullOrEmpty())
                SetCache(configId, cache);

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<T?> GetAboutAsync<T>(string configId) where T : class
        {
            var cache = GetCache(configId);
            if (!cache.IsNullOrEmpty())
                return cache!.ToObject<T>();

            cache = await _repository.GetSelectAsync(x => x.Id == configId, x => x.Value) ?? "";

            if (!cache.IsNullOrEmpty())
                SetCache(configId, cache);

            return cache?.ToObject<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public void RemoveAboutCacheAsync(string configId) => _memoryCache.RemoveMemory(GetCacheKey(configId));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        private string? GetCache(string configId) => _memoryCache.GetMemory<string>(GetCacheKey(configId));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void SetCache(string configId, string value) => _memoryCache.CreateMemory(GetCacheKey(configId), value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        private static string GetCacheKey(string configId) => $"WebAbout:{configId}";
    }
}
