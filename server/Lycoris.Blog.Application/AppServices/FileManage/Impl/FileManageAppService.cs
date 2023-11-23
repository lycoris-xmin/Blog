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
        private readonly IRepository<ServerStaticFile, long> _repository;
        private readonly Lazy<IGithubService> _github;
        private readonly Lazy<IMinioService> _minio;

        public FileManageAppService(IRepository<ServerStaticFile, long> repository, Lazy<IGithubService> github, Lazy<IMinioService> minio)
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

                var data = new ServerStaticFile()
                {
                    Path = $"/{path.TrimStart('/').TrimEnd('/')}",
                    FileName = fileName,
                    UploadChannel = config.UploadChannel,
                    PathUrl = $"/{path.TrimStart('/').TrimEnd('/')}/{fileName}",
                    FileSie = file.Length,
                    Use = true,
                    CreateTime = DateTime.Now
                };

                switch (data.UploadChannel)
                {
                    case FileUploadChannelEnum.Github:
                        data = await GitHubUploadFileAsync(file, data);
                        break;
                    case FileUploadChannelEnum.Minio:
                        data = await MinioUploadFileAsync(file, data);
                        break;
                    case FileUploadChannelEnum.OSS:
                        break;
                    case FileUploadChannelEnum.COS:
                        break;
                    case FileUploadChannelEnum.OBS:
                        break;
                    case FileUploadChannelEnum.Kodo:
                        break;
                    default:
                        break;
                }

                if (data.UploadChannel == FileUploadChannelEnum.Local || config.LocalBackup)
                {
                    var filePath = Path.Combine(AppSettings.Path.WebRootPath, data.Path.TrimStart('/'));

                    FileHelper.CreateDirectory(filePath);

                    await file.SaveAsAsync(Path.Combine(filePath, data.FileName));
                }

                data.LocalBack = config.LocalBackup;

                await _repository.CreateAsync(data);

                var url = AppSettings.Application.HttpPort == 80
                    ? $"{AppSettings.Application.Domain}{data.PathUrl}"
                    : $"{AppSettings.Application.Domain}:{AppSettings.Application.HttpPort}{data.PathUrl}";

                return url.Replace('\\', '/');
            }
            catch (FriendlyException)
            {
                throw;
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
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ServerStaticFile> UploadLocalToRemoteAsync(ServerStaticFile data)
        {
            try
            {
                var filePath = Path.Combine(AppSettings.Path.WebRootPath, data.Path.TrimStart('/'), data.FileName);

                if (!File.Exists(filePath))
                {
                    data.LocalBack = false;
                    await _repository.UpdateFieIdsAsync(data, x => x.LocalBack);
                    throw new FriendlyException("找不到本地备份文件，无法上传");
                }

                var config = await GetConfigurationAsync();

                switch (config.UploadChannel)
                {
                    case FileUploadChannelEnum.Github:
                        data = await GitHubUploadFileAsync(data, filePath);
                        break;
                    case FileUploadChannelEnum.Minio:
                        data = await MinioUploadFileAsync(data, filePath);
                        break;
                    case FileUploadChannelEnum.OSS:
                        break;
                    case FileUploadChannelEnum.COS:
                        break;
                    case FileUploadChannelEnum.OBS:
                        break;
                    case FileUploadChannelEnum.Kodo:
                        break;
                    default:
                        break;
                }

                if (data.UploadChannel != config.UploadChannel)
                    data.UploadChannel = config.UploadChannel;

                await _repository.UpdateFieIdsAsync(data, x => x.UploadChannel, x => x.RemoteUrl, x => x.FileSha);

                return data;
            }
            catch (FriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("upload file failed", ex);
                throw new FriendlyException("同步失败", ex.Message);
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
            data.LastUpdateTime = DateTime.Now;

            await _repository.UpdateFieIdsAsync(data, x => x.Use, x => x.LastUpdateTime!);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<ServerStaticFile> GitHubUploadFileAsync(IFormFile file, ServerStaticFile data)
        {
            data.UploadChannel = FileUploadChannelEnum.Github;

            var (url, sha) = await _github.Value.UploadFileAsync(file, $"{data.Path}/{data.FileName}");

            data.RemoteUrl = url;
            data.FileSha = sha ?? "";

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        private async Task<ServerStaticFile> GitHubUploadFileAsync(ServerStaticFile data, string localFilePath)
        {
            data.UploadChannel = FileUploadChannelEnum.Github;

            var (url, sha) = await _github.Value.UploadFileAsync(localFilePath, $"{data.Path}/{data.FileName}");

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
        private async Task<ServerStaticFile> MinioUploadFileAsync(IFormFile file, ServerStaticFile data)
        {
            data.UploadChannel = FileUploadChannelEnum.Minio;

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
        /// <param name="data"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        private async Task<ServerStaticFile> MinioUploadFileAsync(ServerStaticFile data, string localFilePath)
        {
            data.UploadChannel = FileUploadChannelEnum.Minio;

            data.RemoteUrl = await _minio.Value.UploadFileAsync(x =>
            {
                x.WithBucketPath(data.Path);
                x.WithFile(localFilePath);
            });

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<StaticFileConfiguration> GetConfigurationAsync()
            => await this.ApplicationConfiguration.Value.GetConfigurationAsync<StaticFileConfiguration>(AppConfig.StaticFile) ?? throw new FriendlyException("");
    }
}
