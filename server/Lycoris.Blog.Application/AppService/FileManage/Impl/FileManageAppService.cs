using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.CloudStorage.Minio;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Application.AppService.FileManage.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class FileManageAppService : ApplicationBaseService, IFileManageAppService
    {
        private readonly IRepository<UploadFile, long> _file;
        private readonly Lazy<IMinioService> _minio;

        public FileManageAppService(IRepository<UploadFile, long> file, Lazy<IMinioService> minio)
        {
            _minio = minio;
            _file = file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<FileUploadConfiguration?> CheckMinioUseAsync() => this.ApplicationConfiguration.Value.GetConfigurationAsync<FileUploadConfiguration>(AppConfig.FileUpload);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task CreateUploadFileAsync(string path, string fileName, FileSaveChannelEnum channel)
        {
            var data = new UploadFile()
            {
                Path = path,
                FileName = fileName,
                SaveChannel = channel,
                Use = false,
                CreateTime = DateTime.Now
            };

            await _file.CreateAsync(data);
        }
    }
}
