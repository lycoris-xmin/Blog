using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Common;
using Lycoris.Blog.Common.Extensions;
using Lycoris.Blog.Core.Github;
using Lycoris.Blog.Core.Minio;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Helper;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.FileManage.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class FileManageAppService : ApplicationBaseService, IFileManageAppService
    {
        private readonly IRepository<StaticFiles, long> _repository;
        private readonly Lazy<IGithubService> _github;
        private readonly Lazy<IMinioService> _minio;

        public FileManageAppService(IRepository<StaticFiles, long> repository, Lazy<IGithubService> github, Lazy<IMinioService> minio)
        {
            _repository = repository;
            _github = github;
            _minio = minio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(IFormFile file, string path)
        {
            var config = await GetConfigurationAsync();

            try
            {
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";

                var data = new StaticFiles()
                {
                    Path = $"/{path.TrimStart('/').TrimEnd('/')}",
                    FileName = fileName,
                    SaveChannel = config.SaveChannel,
                    PathUrl = $"/{path.TrimStart('/').TrimEnd('/')}/{fileName}",
                    Use = true,
                    CreateTime = DateTime.Now
                };

                switch (data.SaveChannel)
                {
                    case FileSaveChannelEnum.Github:
                        data = await GitHubUploadFileAsync(file, data);
                        break;
                    case FileSaveChannelEnum.Minio:
                        data = await MinioUploadFileAsync(file, data);
                        break;
                    case FileSaveChannelEnum.OSS:
                        break;
                    case FileSaveChannelEnum.COS:
                        break;
                    case FileSaveChannelEnum.OBS:
                        break;
                    case FileSaveChannelEnum.Kodo:
                        break;
                    default:
                        break;
                }

                if (data.SaveChannel == FileSaveChannelEnum.Local || config.LocalBackup)
                {
                    var filePath = Path.Combine(AppSettings.Path.WebRootPath, data.Path.TrimStart('/'));

                    FileHelper.CreateDirectory(filePath);

                    await file.SaveAsAsync(Path.Combine(filePath, data.FileName));
                }

                await _repository.CreateAsync(data);

                return AppSettings.Application.HttpPort == 80
                    ? $"{AppSettings.Application.Domain}{data.PathUrl}"
                    : $"{AppSettings.Application.Domain}:{AppSettings.Application.HttpPort}{data.PathUrl}";
            }
            catch (Exception ex)
            {
                _logger.Error("upload file failed", ex);
                throw new FriendlyException("上传失败", ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathUrl"></param>
        /// <returns></returns>
        public async Task SetFileDeleteAsync(string pathUrl)
        {
            pathUrl = AppSettings.Application.HttpPort == 80
                ? pathUrl.Replace(AppSettings.Application.Domain, "")
                : pathUrl.Replace($"{AppSettings.Application.Domain}:{AppSettings.Application.HttpPort}", "");

            var data = await _repository.GetAsync(x => x.PathUrl == pathUrl);
            if (data == null)
                return;

            data.Use = false;

            await _repository.UpdateFieIdsAsync(data, x => x.Use);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<StaticFiles> GitHubUploadFileAsync(IFormFile file, StaticFiles data)
        {
            data.SaveChannel = FileSaveChannelEnum.Github;

            var (url, sha) = await _github.Value.UploadFileAsync(file, $"{data.Path}/{data.FileName}");

            data.RemoteUrl = url;
            data.FileSha = sha ?? "";

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<StaticFiles> MinioUploadFileAsync(IFormFile file, StaticFiles data)
        {
            data.SaveChannel = FileSaveChannelEnum.Minio;

            data.RemoteUrl = await _minio.Value.UploadFileAsync(x =>
            {
                x.WithBucketPath(data.Path);
                x.WithFormFile(file);
            });

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<FileUploadConfiguration> GetConfigurationAsync()
            => await this.ApplicationConfiguration.Value.GetConfigurationAsync<FileUploadConfiguration>(AppConfig.FileUpload) ?? throw new FriendlyException("");
    }
}
