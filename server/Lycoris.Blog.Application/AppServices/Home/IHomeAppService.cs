using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.Application.Shared;

namespace Lycoris.Blog.Application.AppServices.Home
{
    public interface IHomeAppService : IApplicationBaseService
    {
        /// <summary>
        /// 网站配置信息
        /// </summary>
        /// <returns></returns>
        Task<WebSettingDto> GetWebSettingsAsync();

        /// <summary>
        /// 站长信息
        /// </summary>
        /// <returns></returns>
        Task<WebOwnerDto> GetWebOwnerAsync();

        /// <summary>
        /// 关于本站
        /// </summary>
        /// <returns></returns>
        Task<string> GetAboutWebAsync();

        /// <summary>
        /// 关于我
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
        /// 网站发布统计
        /// </summary>
        /// <returns></returns>
        Task<PublishStatisticsDto> GetPublishStatisticsAsync();

        /// <summary>
        /// 网站互动统计
        /// </summary>
        /// <returns></returns>
        Task<InteractiveStatisticsDto> GetInteractiveStatisticsAsync();

        /// <summary>
        /// 文章分类统计
        /// </summary>
        /// <returns></returns>
        Task<List<CategoryStatisticsDto>> GetCategoryStatisticsAsync();

        /// <summary>
        /// 文章随机图
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetPostIconAsync();
    }
}
