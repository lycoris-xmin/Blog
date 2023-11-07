using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common.Cache;

namespace Lycoris.Blog.Application.Cached.StaticFiles.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class StaticFilesCacheService : IStaticFilesCacheService
    {
        private readonly IMemoryCacheManager _memoryCache;

        public StaticFilesCacheService(IMemoryCacheManager memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void SetStaticFileUse(string fileName) => _memoryCache.CreateMemory(GetCacheKey(fileName), "1");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetStaticFileUse(string fileName)
        {
            var cache = _memoryCache.GetMemory<string>(GetCacheKey(fileName));
            return cache == "1";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetCacheKey(string fileName) => $"CheckFileUser:{fileName}";
    }
}
