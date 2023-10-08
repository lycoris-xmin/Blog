using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.Configurations
{
    public interface IConfigurationAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<string?> GetConfigurationAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveConfigurationAsync<T>(string configId, T value) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<EnumsDto<int>> GetSaveChannelEnum();
    }
}
