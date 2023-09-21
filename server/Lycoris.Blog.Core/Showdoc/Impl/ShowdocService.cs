using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Http;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Core.Repositories;
using Lycoris.Blog.Core.Showdoc.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Core.Showdoc.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class ShowdocService : IShowdocService
    {
        private ILycorisLogger _logger;
        private readonly IConfigurationRepository _configuration;

        public ShowdocService(ILycorisLoggerFactory factory, IConfigurationRepository configuration)
        {
            _logger = factory.CreateLogger<ShowdocService>();
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task PublishAsync(string title, string content)
        {
            var host = await GetShowdocConfigurationAsync();
            if (host.IsNullOrEmpty())
            {
                _logger.Warn($"showdoc publish -> the showdoc host can not found");
                return;
            }

            var request = new HttpUtils(host);

            var body = new ShowdocPublishDataModel()
            {
                Title = title,
                Content = content
            }.ToJson();

            request.AddJsonBody(body);

            _logger.Info($"showdoc publish -> request:{host} Body:{body}");

            var response = await request.HttpPostAsync();

            if (!response.Success || response.Content.IsNullOrEmpty())
            {
                _logger.Error($"showdoc publish -> request failed:{(response.Exception != null ? $"{response.Exception.Message}\r\n{response.Exception.StackTrace}" : response.ToJson())}");
                return;
            }

            _logger.Info($"showdoc publish -> response:{response.Content}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public Task PublishAsync(ShowdocTemplate template)
        {
            var content = template.BuildMarndownContent();
            return PublishAsync(template.Title ?? "系统通知", content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetShowdocConfigurationAsync()
        {
            var settings = await _configuration.GetConfigurationAsync<OtherSettingsConfiguration>(AppConfig.OtherSettings);
            return settings?.ShowDocHost ?? "";
        }
    }
}
