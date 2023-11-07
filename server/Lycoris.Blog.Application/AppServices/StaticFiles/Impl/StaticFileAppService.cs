using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.StaticFiles.Dtos;
using Lycoris.Blog.Application.Cached.StaticFiles;
using Lycoris.Blog.Application.Schedule.Jobs;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.StaticFiles.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class StaticFileAppService : ApplicationBaseService, IStaticFileAppService
    {
        private readonly IRepository<StaticFile, long> _repository;
        private readonly Lazy<IStaticFilesCacheService> _cache;
        private readonly Lazy<IQuartzSchedulerCenter> _schedulerCenter;
        private readonly Lazy<IFileManageAppService> _fileManage;

        public StaticFileAppService(IRepository<StaticFile, long> repository,
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
                                  FileName = x.FileName,
                                  UploadChannel = x.UploadChannel,
                                  PathUrl = x.PathUrl,
                                  RemoteUrl = x.RemoteUrl,
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

            await _schedulerCenter.Value.AddOnceJobAsync<CheckFileUseStateJob>(id.ToString());

            _cache.Value.SetStaticFileUse(data.FileName);
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

            try
            {
                await _fileManage.Value.UploadLocalToRemoteAsync(data);
            }
            catch (GitHubFileException ex)
            {
                throw new OutputException(ResCodeEnum.RemoteFileRepeat, ex.Message);
            }
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
    }
}
