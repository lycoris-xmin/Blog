using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
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
        public Task<WebSettingsConfiguration?> GetWebSettingsAsync() => ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSetting);

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
        public Task<T?> GetAboutMeAsync<T>(string configName) where T : class => _webAbout.Value.GetAboutAsync<T>(configName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WebBrowseRecordAsync(WebBrowseRecordDto input)
        {
            var repository = _provider.GetRequiredService<IRepository<BrowseLog, long>>();

            var addr = IPAddressHelper.Search(CurrentRequest.RequestIP);

            var data = new BrowseLog()
            {
                ClientOrign = input.ClientOrign,
                Path = (input.Path != "/" ? input.Path.TrimEnd('/') : input.Path).Trim(),
                PageName = input.PageName,
                UserAgent = CurrentRequest.UserAgent,
                Referer = (input.Referer ?? "").TrimEnd('/').Trim(),
                Ip = IPAddressHelper.Ipv4ToUInt32(CurrentRequest.RequestIP),
                Country = addr.IsPrivate ? "中国" : (addr.Country ?? ""),
                IpAddress = IPAddressHelper.ChangeAddress(addr),
                CreateTime = DateTime.Now
            };

            var checkTime = data.CreateTime.AddMinutes(1);
            var hasRecord = await repository.GetAll()
                                            .Where(x => x.CreateTime >= checkTime && x.ClientOrign == input.ClientOrign)
                                            .Where(x => x.Path == data.Path)
                                            .Where(x => x.UserAgent == data.UserAgent)
                                            .Where(x => x.Ip == data.Ip)
                                            .AnyAsync();

            if (!hasRecord)
            {
                await repository.CreateAsync(data);
                _provider.GetRequiredService<IScheduleQueueCacheService>().Enqueue(ScheduleTypeEnum.BrowseLog, new BrowseLogQueueModel(data.Path, data.Referer, CurrentRequest.RequestIP));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<OwnerCreateStatisticsDto> GetOwnerCreateStatisticsAsync()
        {
            return new OwnerCreateStatisticsDto()
            {
                Post = await _provider.GetRequiredService<IRepository<Post, long>>().GetAll().Where(x => x.IsPublish == true).CountAsync(),
                Talk = await _provider.GetRequiredService<IRepository<Talk, long>>().GetAll().CountAsync(),
                Category = await _provider.GetRequiredService<IRepository<Category, int>>().GetAll().CountAsync()
            };
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetPostIconAsync()
        {
            var config = await this.ApplicationConfiguration.Value.GetConfigurationAsync<PostSettingConfiguration>(AppConfig.PostSetting);
            return config?.Images ?? new List<string>();
        }
    }
}
