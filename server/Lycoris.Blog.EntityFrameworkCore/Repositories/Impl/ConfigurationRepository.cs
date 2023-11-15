using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common.Cache;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Lycoris.Blog.EntityFrameworkCore.Repositories.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IRepository<Configuration, string> _repository;
        private readonly IMemoryCacheManager _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="memoryCache"></param>
        public ConfigurationRepository(IRepository<Configuration, string> repository, IMemoryCacheManager memoryCache)
        {
            _repository = repository;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<Configuration?> GetDataAsync(string configId) => _repository.GetAsync(x => x.Id == configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<string> GetConfigurationAsync(string configId)
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
        public async Task<T?> GetConfigurationAsync<T>(string configId) where T : class
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
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SaveConfigurationAsync(string configId, string value)
        {
            var data = await GetDataAsync(configId);
            if (data == null)
                return;

            data.Value = value;
            await _repository.UpdateFieIdsAsync(data, x => x.Value);

            RemoveConfigurationCache(configId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SaveConfigurationAsync<T>(string configId, [NotNull] T value) where T : class
        {
            var data = await GetDataAsync(configId);
            if (data == null)
                return;

            data.Value = value.ToJson();
            await _repository.UpdateFieIdsAsync(data, x => x.Value);

            RemoveConfigurationCache(configId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SaveConfigurationAsync(Configuration value)
        {
            await _repository.UpdateAsync(value);
            RemoveConfigurationCache(value.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public void RemoveConfigurationCache(string configId) => _memoryCache.RemoveMemory(GetCacheKey(configId));

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
        private static string GetCacheKey(string configId) => $"Configuration:{configId}";
    }
}
