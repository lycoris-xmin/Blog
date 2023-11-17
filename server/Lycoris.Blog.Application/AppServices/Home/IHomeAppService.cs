using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Application.AppServices.Home
{
    public interface IHomeAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<WebSettingsConfiguration?> GetWebSettingsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<WebOwnerDto> GetWebOwnerAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> GetAboutWebAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        Task<T?> GetAboutMeAsync<T>(string configName) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task WebBrowseRecordAsync(WebBrowseRecordDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<OwnerCreateStatisticsDto> GetOwnerCreateStatisticsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<WebStatisticsDto> GetWebStatisticsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<PostStatisticsDto>> GetPostStatisticsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetPostIconAsync();
    }
}
