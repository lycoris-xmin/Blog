using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Application.AppService.Home.Dtos;
using Lycoris.Blog.Application.Cached.ScheduleQueueCache;
using Lycoris.Blog.Application.Cached.ScheduleQueueCache.Dtos;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Blog.Application.AppService.Home.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class HomeAppService : ApplicationBaseService, IHomeAppService
    {
        private readonly IServiceProvider _provider;

        public HomeAppService(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<WebSettingsConfiguration?> GetWebSettingsAsync() => this.ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSettings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<WebOwnerDto> GetWebOwnerAsync()
        {
            var userRepository = this._provider.GetRequiredService<IRepository<User, long>>();
            var user = await userRepository.GetAsync(TableSeedData.UserData.Id);

            var dto = user!.ToMap<WebOwnerDto>();

            var userLinkRepository = this._provider.GetRequiredService<IRepository<UserLink, long>>();

            var link = await userLinkRepository.GetAsync(TableSeedData.UserData.Id);

            dto.QQ = link?.QQ;
            dto.Wechat = link?.WeChat;
            dto.Github = link?.Github;
            dto.CloudMusic = link?.CloudMusic;
            dto.Bilibili = link?.Bilibili;

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> GetAboutWebAsync() => this.ApplicationConfiguration.Value.GetConfigurationAsync(AppConfig.AboutWeb);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        public Task<T?> GetAboutMeAsync<T>(string configName) where T : class => this.ApplicationConfiguration.Value.GetConfigurationAsync<T>(configName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WebBrowseRecordAsync(WebBrowseRecordDto input)
        {
            var repository = this._provider.GetRequiredService<IRepository<BrowseLog, long>>();

            var data = new BrowseLog()
            {
                ClientOrign = input.ClientOrign,
                Path = (input.Path != "/" ? input.Path.TrimEnd('/') : input.Path).Trim(),
                PageName = input.PageName,
                UserAgent = CurrentRequest.UserAgent,
                Referer = (input.Referer ?? "").TrimEnd('/').Trim(),
                Ip = IPAddressHelper.Ipv4ToUInt32(CurrentRequest.RequestIP),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(CurrentRequest.RequestIP)),
                CreateTime = DateTime.Now
            };

            var checkTime = data.CreateTime.AddMinutes(-30);
            var hasRecord = await repository.GetAll()
                                            .Where(x => x.CreateTime >= checkTime && x.ClientOrign == input.ClientOrign)
                                            .Where(x => x.Path == data.Path)
                                            .Where(x => x.UserAgent == data.UserAgent)
                                            .Where(x => x.Ip == data.Ip)
                                            .AnyAsync();

            if (!hasRecord)
            {
                await repository.CreateAsync(data);

                if (!data.Referer.IsNullOrEmpty())
                {
                    await this._provider.GetRequiredService<IScheduleQueueCacheService>().EnqueueAsync(ScheduleTypeEnum.BrowseLog, new BrowseLogQueueDto(data.Path, data.Referer));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<WebStatisticsDto> GetWebStatisticsAsync()
        {
            return new WebStatisticsDto
            {
                Browse = await this._provider.GetRequiredService<IRepository<WebDayStatistics, int>>().GetAll().SumAsync(x => x.PVBrowse),
                Comment = await this._provider.GetRequiredService<IRepository<PostComment, long>>().GetAll().CountAsync(),
                Message = await this._provider.GetRequiredService<IRepository<LeaveMessage, int>>().GetAll().CountAsync()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<PostStatisticsDto>> GetPostStatisticsAsync()
        {
            var query = this._provider.GetRequiredService<IRepository<Category, int>>().GetAll().Select(x => new PostStatisticsDto()
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.PostCount
            });

            var list = await query.ToListAsync() ?? new List<PostStatisticsDto>();

            var count = await this._provider.GetRequiredService<IRepository<Post, long>>().GetAll().Where(x => x.Category <= 0).CountAsync();
            if (count > 0)
            {
                list.Add(new PostStatisticsDto()
                {
                    Id = 0,
                    Name = "未分类",
                    Value = count
                });
            }

            return list;
        }
    }
}
