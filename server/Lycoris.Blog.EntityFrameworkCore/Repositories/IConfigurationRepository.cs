using Lycoris.Blog.EntityFrameworkCore.Tables;

namespace Lycoris.Blog.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<Configuration?> GetDataAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<string> GetConfigurationAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<T?> GetConfigurationAsync<T>(string configId) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        void RemoveConfigurationCacheAsync(string configId);
    }
}
