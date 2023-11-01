﻿using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.WebSiteAbouts.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class WebSiteAboutAppService : ApplicationBaseService, IWebSiteAboutAppService
    {
        private readonly IRepository<WebSiteAbout, string> _repository;
        private readonly Lazy<IWebSiteAboutRepository> _webAboutRepository;

        public WebSiteAboutAppService(IRepository<WebSiteAbout, string> repository, Lazy<IWebSiteAboutRepository> webAboutRepository)
        {
            _repository = repository;
            _webAboutRepository = webAboutRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<string?> GetAboutAsync(string configId) => _repository.GetAll().Where(x => x.Id == configId).Select(x => x.Value).SingleOrDefaultAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<T?> GetAboutAsync<T>(string configId) where T : class
        {
            var val = await GetAboutAsync(configId);
            return val?.ToObject<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveAboutAsync(string configId, string value) => UpdateAsync(configId, value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveAboutAsync<T>(string configId, T value) where T : class => UpdateAsync(configId, value?.ToJson() ?? "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private async Task UpdateAsync(string configId, string value)
        {
            var data = await _repository.GetAsync(configId) ?? throw new FriendlyException("");

            if (data.Value != value)
            {
                // 移除缓存
                _webAboutRepository.Value.RemoveAboutCacheAsync(data.Id);

                data.Value = value;
                await _repository.UpdateFieIdsAsync(data, x => x.Value);
            }
        }
    }
}
