using Lycoris.Blog.EntityFrameworkCore.Tables;
using System.Diagnostics.CodeAnalysis;

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
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveConfigurationAsync(string configId, string value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveConfigurationAsync<T>(string configId, [NotNull] T value) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveConfigurationAsync(Configuration value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        void RemoveConfigurationCache(string configId);
    }
}
