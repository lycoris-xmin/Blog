using Lycoris.Blog.Application.Shared;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.WebSite
{
    public interface IWebSiteAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<string?> GetAboutAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<T?> GetAboutAsync<T>(string configId) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveAboutAsync(string configId, string value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveAboutAsync<T>(string configId, T value) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file, string remotePath);
    }
}
