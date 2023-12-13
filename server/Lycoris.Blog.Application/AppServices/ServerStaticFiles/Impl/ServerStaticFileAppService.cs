using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.ServerStaticFiles.Dtos;
using Lycoris.Blog.Application.Cached.StaticFiles;
using Lycoris.Blog.Application.Schedule.Jobs;
using Lycoris.Blog.Application.Schedule.Models;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.ServerStaticFiles.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class ServerStaticFileAppService : ApplicationBaseService, IServerStaticFileAppService
    {
        private readonly IRepository<ServerStaticFile, long> _repository;
        private readonly Lazy<IStaticFilesCacheService> _cache;
        private readonly Lazy<IQuartzSchedulerCenter> _schedulerCenter;
        private readonly Lazy<IFileManageAppService> _fileManage;

        public ServerStaticFileAppService(IRepository<ServerStaticFile, long> repository,
                                    Lazy<IStaticFilesCacheService> cache,
                                    Lazy<IQuartzSchedulerCenter> schedulerCenter,
                                    Lazy<IFileManageAppService> fileManage)
        {
            _repository = repository;
            _cache = cache;
            _schedulerCenter = schedulerCenter;
            _fileManage = fileManage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<StaticFileDataDto>> GetListAsync(StaticFileListFilter input)
        {
            var filter = _repository.GetAll()
                                    .WhereIf(input.BeginTime.HasValue, x => x.CreateTime >= input.BeginTime!.Value)
                                    .WhereIf(input.EndTime.HasValue, x => x.CreateTime <= input.EndTime!.Value)
                                    .WhereIf(input.UploadChannel.HasValue, x => x.UploadChannel == input.UploadChannel!.Value)
                                    .WhereIf(input.LocalBack.HasValue, x => x.LocalBack == input.LocalBack!.Value)
                                    .WhereIf(input.Use.HasValue, x => x.Use == input.Use!.Value);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<StaticFileDataDto>(count);

            var query = filter.OrderByDescending(x => x.Id)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new StaticFileDataDto
                              {
                                  Id = x.Id,
                                  Path = x.Path,
                                  FileName = x.FileName,
                                  UploadChannel = x.UploadChannel,
                                  PathUrl = x.PathUrl,
                                  RemoteUrl = x.RemoteUrl,
                                  FileType = x.FileType,
                                  FileSize = x.FileSie,
                                  FileSha = x.FileSha,
                                  LocalBack = x.LocalBack,
                                  Use = x.Use,
                                  CreateTime = x.CreateTime
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<StaticFileDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task CheckFileUseStateAsync(long id)
        {
            var data = await _repository.GetAsync(id) ?? throw new FriendlyException("数据不存在或已被删除");

            if (_cache.Value.GetStaticFileUse(data.FileName))
                return;

            try
            {
                await _schedulerCenter.Value.AddOnceJobAsync<CheckFileUseStateJob>(id.ToString());
                _cache.Value.SetStaticFileUse(data.FileName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("job name") && ex.Message.Contains("already exists"))
                    throw new FriendlyException("无法同时执行太多检测任务，请稍候再试");

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SyncFileToRemoteRepositoryAsync(long id)
        {
            var data = await _repository.GetAsync(id) ?? throw new FriendlyException("数据不存在或已被删除");
            if (!data.LocalBack)
                throw new FriendlyException("该文件本地没有备份，无法上传");

            await _fileManage.Value.UploadLocalToRemoteAsync(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task UploadLocalFileAsync(long id, IFormFile file)
        {
            var data = await _repository.GetAsync(id) ?? throw new FriendlyException("数据不存在或已被删除");

            var filePath = Path.Combine(AppSettings.Path.WebRootPath, data.Path.TrimStart('/'), data.FileName);
            if (File.Exists(filePath))
                return;

            await file.OpenReadStream().SaveAsAsync(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteLocalFileAsync(long id)
        {
            var data = await _repository.GetAsync(id) ?? throw new FriendlyException("数据不存在或已被删除");

            var filePath = Path.Combine(AppSettings.Path.WebRootPath, data.Path.TrimStart('/'), data.FileName);
            if (!File.Exists(filePath))
                return;

            File.Delete(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> DownloadAllFileAsync()
        {
            var zipFileName = $"staticfile-{DateTime.Now:yyyyMMddHHmmss}.zip";
            var zipFilePath = Path.Combine(AppSettings.Path.Temp, zipFileName);

            if (!Directory.Exists(AppSettings.Path.Temp))
                Directory.CreateDirectory(AppSettings.Path.Temp);

            await _schedulerCenter.Value.AddOnceJobAsync<BackupFileJob, BackupFileJobModel>(new BackupFileJobModel()
            {
                SourceFilePath = AppSettings.Path.WebRootPath,
                ZipFilePath = zipFilePath
            });

            return Path.GetFileName(zipFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public async Task<PageResultDto<ServerStaticFileRepositoryDto>> GetServerStaticFileRepositoryAsync(int pageIndex, int pageSize, FileTypeEnum? fileType)
        {
            var filter = _repository.GetAll().WhereIf(fileType.HasValue, x => x.FileType == fileType!.Value);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<ServerStaticFileRepositoryDto>(count);

            var query = filter.OrderByDescending(x => x.CreateTime)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new ServerStaticFileRepositoryDto()
                              {
                                  Url = x.PathUrl,
                                  FileName = x.FileName,
                                  FileSize = x.FileSie,
                                  FileType = x.FileType
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<ServerStaticFileRepositoryDto>(count, list);
        }
    }
}
