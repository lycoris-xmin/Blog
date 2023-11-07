using Lycoris.Autofac.Extensions;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Octokit;
using Octokit.Internal;

namespace Lycoris.Blog.Core.Github.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class GithubService : IGithubService
    {
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
        public async Task<(string url, string? sha)> UploadFileAsync(string localPath, string remotePath)
        {
            var content = GetFileToBase64(localPath);

            if (content.IsNullOrEmpty())
                throw new FriendlyException("");

            var config = await GetConfigurationAsync();

            var (owner, repo) = config.AnalyzeRepository();

            var client = CreateGitHubClient(config);

            try
            {
                var res = await client.Repository.Content.CreateFile(owner, repo, remotePath.TrimStart('/'), new CreateFileRequest("upload file", content, false));

                var url = config.ChangeJsDelivrCDNUrl(owner, repo, res.Content!.Path);

                return (url, res.Content!.Sha);
            }
            catch (ApiValidationException ex)
            {
                if (ex.Message.Contains("\"sha\" wasn't supplied"))
                    throw new GitHubFileException(UploadFileResultEnum.Repeat, "文件已存在远端仓库");

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<(string url, string? sha)> UploadFileAsync(IFormFile file, string remotePath)
        {
            var content = GetFileToBase64(file);

            if (content.IsNullOrEmpty())
                throw new FriendlyException("");

            var config = await GetConfigurationAsync();

            var (owner, repo) = config.AnalyzeRepository();

            var client = CreateGitHubClient(config);

            try
            {
                var res = await client.Repository.Content.CreateFile(owner, repo, remotePath.TrimStart('/'), new CreateFileRequest("upload file", content, false));

                var url = config.ChangeJsDelivrCDNUrl(owner, repo, res.Content!.Path);

                return (url, res.Content!.Sha);
            }
            catch (ApiValidationException ex)
            {
                if (ex.Message.Contains("\"sha\" wasn't supplied"))
                    throw new GitHubFileException(UploadFileResultEnum.Repeat, "文件已存在远端仓库");

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async Task<byte[]?> GetFileAsync(string remotePath)
        {
            var config = await GetConfigurationAsync();

            var (owner, repo) = config.AnalyzeRepository();

            var client = CreateGitHubClient(config);

            return await client.Repository.Content.GetRawContent(owner, repo, remotePath.TrimStart('/'));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sha"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async Task RemoveFileAsync(string sha, string remotePath)
        {
            var config = await GetConfigurationAsync();

            var (owner, repo) = config.AnalyzeRepository();

            var client = CreateGitHubClient(config);

            await client.Repository.Content.DeleteFile(owner, repo, remotePath.TrimStart('/'), new DeleteFileRequest("delete file", sha));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<GithubConfiguration> GetConfigurationAsync()
        {
            var config = await _configuration.GetConfigurationAsync<StaticFileConfiguration>(AppConfig.StaticFile) ?? throw new FriendlyException("");

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
            if (!File.Exists(filePath))
                throw new FriendlyException("上传失败", $"can not find file with path:{filePath}");

            using var fs = new FileStream(filePath, System.IO.FileMode.Open);
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
        /// <param name="config"></param>
        /// <returns></returns>
        private static GitHubClient CreateGitHubClient(GithubConfiguration config)
        {
            var credentials = new InMemoryCredentialStore(new Credentials(config.AccessToken));
            return new GitHubClient(new ProductHeaderValue(config.CommitterName), credentials);
        }
    }
}
