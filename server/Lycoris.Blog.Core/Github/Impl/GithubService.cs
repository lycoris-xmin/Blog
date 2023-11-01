using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Core.Github.Models;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Common.Http;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Core.Github.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class GithubService : IGithubService
    {
        const string Url = "https://api.github.com";

        private readonly IConfigurationRepository _configuration;

        public GithubService(IConfigurationRepository configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task UploadFileAsync(string localPath, string remotePath)
        {
            var content = GetFileToBase64(localPath);

            if (content.IsNullOrEmpty())
                throw new FriendlyException("");

            var config = await GetConfigurationAsync();

            await PutRequestAsync(config, content, remotePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task UploadFileAsync(IFormFile file, string remotePath)
        {
            var content = GetFileToBase64(file);

            if (content.IsNullOrEmpty())
                throw new FriendlyException("");

            var config = await GetConfigurationAsync();

            await PutRequestAsync(config, content, remotePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<GithubConfiguration> GetConfigurationAsync()
        {
            var config = await _configuration.GetConfigurationAsync<FileUploadConfiguration>(AppConfig.FileUpload) ?? throw new FriendlyException("");

            if (config.Github == null)
                throw new FriendlyException("");

            if (config.Github.AccessToken.IsNullOrEmpty())
                throw new FriendlyException("");
            else if (config.Github.RepositoryUrl.IsNullOrEmpty())
                throw new FriendlyException("");

            return config.Github;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetFileToBase64(string filePath)
        {
            if (File.Exists(filePath))
                return "";

            using var fs = new FileStream(filePath, FileMode.Open);
            var bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetFileToBase64(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        private static async Task PutRequestAsync(GithubConfiguration config, string content, string? remotePath)
        {
            var (owner, repo) = AnalyzeRepository(config.RepositoryUrl);

            remotePath ??= config.RepositoryPath;

            var request = new HttpUtils($"{Url}/repos/{owner}/{repo}/contents/{remotePath}");

            var body = $"'{{\"message\":\"upload file\",\"committer\":{{\"name\":\"{config.CommitterName}\",\"email\":\"{config.CommitterEmail}\"}},\"content\":\"{content}\"}}'";

            request.AddJsonBody(body);
            request.AddRequestHeader("Accept", "application/vnd.github+json");
            request.AddRequestHeader("Authorization", $"Bearer {config.AccessToken}");
            request.AddRequestHeader("X-GitHub-Api-Version", DateTime.Now.ToString("yyyy-MM-dd"));

            var res = await request.HttpPutAsync();

            if (!res.Success)
                throw new FriendlyException("");

            if (res.Content.IsNullOrEmpty())
                throw new FriendlyException("");

            var data = res.Content.ToObject<GithubPutFileResponse>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repoUrl"></param>
        /// <returns></returns>
        private static (string owner, string repo) AnalyzeRepository(string repoUrl)
        {
            var url = repoUrl.Replace("https://github.com/", "");

            var paths = url.Split('/');

            if (paths.Length != 2)
                throw new FriendlyException("");

            return (paths[0], paths[1]);
        }
    }
}
