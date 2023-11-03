using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.StaticFiles.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions.Services.Impl;
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

        public StaticFileAppService(IRepository<StaticFile, long> repository)
        {
            _repository = repository;
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
                                    .WhereIf(input.SaveChannel.HasValue, x => x.SaveChannel <= input.SaveChannel!.Value)
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
                                  SaveChannel = x.SaveChannel,
                                  PathUrl = x.PathUrl,
                                  RemoteUrl = x.RemoteUrl,
                                  FileSha = x.FileSha,
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
        /// <param name="use"></param>
        /// <returns></returns>
        public async Task SetFileUseStateAsync(long id, bool use)
        {
            var data = await _repository.GetAsync(id) ?? throw new FriendlyException("");
            if (data.Use != use)
            {
                data.Use = use;
                await _repository.UpdateFieIdsAsync(data, x => x.Use);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SyncFileToRemoteRepositoryAsync(long id)
        {
            var center = new QuartzSchedulerCenter();

            center.AddJobAsync();
        }
    }
}
