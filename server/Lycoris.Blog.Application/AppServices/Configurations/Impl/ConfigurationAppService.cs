using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.Configurations.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class ConfigurationAppService : ApplicationBaseService, IConfigurationAppService
    {
        private readonly IRepository<Configuration, int> _configuration;

        public ConfigurationAppService(IRepository<Configuration, int> configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<string?> GetConfigurationAsync(string configId) => _configuration.GetAll().Where(x => x.ConfigId == configId).Select(x => x.Value).SingleOrDefaultAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<T?> GetConfigurationAsync<T>(string configId) where T : class
        {
            var val = await GetConfigurationAsync(configId);
            return val?.ToObject<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveConfigurationAsync(string configId, string value) => UpdateAsync(configId, value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveConfigurationAsync<T>(string configId, T value) where T : class => UpdateAsync(configId, value?.ToJson() ?? "");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<EnumsDto<int>> GetFileSaveChannelEnum()
        {
            return Enum.GetValues<FileSaveChannelEnum>().Select(x => new EnumsDto<int>
            {
                Value = (int)x,
                Name = x.GetEnumDescription<FileSaveChannelEnum>()
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private async Task UpdateAsync(string configId, string value)
        {
            var data = await _configuration.GetAsync(x => x.ConfigId == configId) ?? throw new FriendlyException("");

            if (data.Value != value)
            {
                // 移除缓存
                ApplicationConfiguration.Value.RemoveConfigurationCacheAsync(data.ConfigId);

                data.Value = value;
                await _configuration.UpdateFieIdsAsync(data, x => x.Value);
            }
        }
    }
}
