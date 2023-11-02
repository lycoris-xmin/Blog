using Lycoris.Blog.Model.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Core.Github
{
    public interface IGithubService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        Task<(string url, string? sha)> UploadFileAsync(string localPath, string remotePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        Task<(string url, string? sha)> UploadFileAsync(IFormFile file, string remotePath);
    }
}
