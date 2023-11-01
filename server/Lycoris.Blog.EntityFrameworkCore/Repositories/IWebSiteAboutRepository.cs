using Lycoris.Blog.EntityFrameworkCore.Tables;

namespace Lycoris.Blog.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWebSiteAboutRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<WebSiteAbout?> GetDataAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<string> GetAboutAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<T?> GetAboutAsync<T>(string configId) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        void RemoveAboutCacheAsync(string configId);
    }
}
