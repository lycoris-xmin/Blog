using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Cache.ScheduleQueue;
using Lycoris.Blog.Cache.ScheduleQueue.Models;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Blog.Application.AppServices.Home.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class HomeAppService : ApplicationBaseService, IHomeAppService
    {
        private readonly IServiceProvider _provider;
        private readonly Lazy<IWebSiteAboutRepository> _webAbout;

        public HomeAppService(IServiceProvider provider, Lazy<IWebSiteAboutRepository> webAbout)
        {
            _provider = provider;
            _webAbout = webAbout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<WebSettingsConfiguration?> GetWebSettingsAsync() => ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSettings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<WebOwnerDto> GetWebOwnerAsync()
        {
            var userRepository = _provider.GetRequiredService<IRepository<User, long>>();
            var user = await userRepository.GetAsync(TableSeedData.UserData.Id);

            var dto = user!.ToMap<WebOwnerDto>();

            var userLinkRepository = _provider.GetRequiredService<IRepository<UserLink, long>>();

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
        public Task<string> GetAboutWebAsync() => _webAbout.Value.GetAboutAsync(AppAbout.AboutWeb);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        public Task<T?> GetAboutMeAsync<T>(string configName) where T : class => ApplicationConfiguration.Value.GetConfigurationAsync<T>(configName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WebBrowseRecordAsync(WebBrowseRecordDto input)
        {
            var repository = _provider.GetRequiredService<IRepository<BrowseLog, long>>();

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
                    _provider.GetRequiredService<IScheduleQueueCacheService>().Enqueue(ScheduleTypeEnum.BrowseLog, new BrowseLogQueueModel(data.Path, data.Referer));
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
                Browse = await _provider.GetRequiredService<IRepository<WebDayStatistics, int>>().GetAll().SumAsync(x => x.PVBrowse),
                Comment = await _provider.GetRequiredService<IRepository<PostComment, long>>().GetAll().CountAsync(),
                Message = await _provider.GetRequiredService<IRepository<LeaveMessage, int>>().GetAll().CountAsync()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<PostStatisticsDto>> GetPostStatisticsAsync()
        {
            var query = _provider.GetRequiredService<IRepository<Category, int>>().GetAll().Select(x => new PostStatisticsDto()
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.PostCount
            });

            var list = await query.ToListAsync() ?? new List<PostStatisticsDto>();

            var count = await _provider.GetRequiredService<IRepository<Post, long>>().GetAll().Where(x => x.Category <= 0).CountAsync();
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
