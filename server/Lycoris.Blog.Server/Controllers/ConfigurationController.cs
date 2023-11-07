using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Configurations;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Configurations;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Configuration"), AppAuthentication]
    public class ConfigurationController : BaseController
    {
        private readonly IConfigurationAppService _configuration;
        private readonly Lazy<IFileManageAppService> _fileManage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileManage"></param>
        public ConfigurationController(IConfigurationAppService configuration, Lazy<IFileManageAppService> fileManage)
        {
            _configuration = configuration;
            _fileManage = fileManage;
        }

        /// <summary>
        /// 获取网站基础配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web")]
        [Produces("application/json")]
        public async Task<DataOutput<WebSettingsConfiguration>> WebSettings()
        {
            var dto = await _configuration.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSettings);
            return Success(dto);
        }

        /// <summary>
        /// 保存网站基础配置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Web")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveWebSettings([FromBody] SaveWebSettingsInput input)
        {
            var config = await _configuration.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSettings);

            if (!input.WebName.IsNullOrEmpty())
                config!.WebName = input.WebName!;
            if (!input.WebPath.IsNullOrEmpty())
                config!.WebPath = input.WebPath!;
            if (!input.AdminPath.IsNullOrEmpty())
                config!.AdminPath = input.AdminPath!;
            if (input.BuildTime.HasValue)
                config!.BuildTime = input.BuildTime!.Value;

            await _configuration.SaveConfigurationAsync(AppConfig.WebSettings, config!);
            return Success();
        }

        /// <summary>
        /// 获取博客相关设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Post")]
        [Produces("application/json")]
        public async Task<DataOutput<PostSettingConfiguration>> PostSettings()
        {
            var dto = await _configuration.GetConfigurationAsync<PostSettingConfiguration>(AppConfig.PostSettings);
            return Success(dto);
        }

        /// <summary>
        /// 保存博客相关设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Post")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SavePostSettings([FromBody] SavePostSettingsInput input)
        {
            var config = await _configuration.GetConfigurationAsync<PostSettingConfiguration>(AppConfig.PostSettings)!;

            config!.AutoSave = input.AutoSave!.Value;
            if (input.Second.HasValue && input.Second > 0)
                config.Second = input.Second!.Value;
            if (input.Images.HasValue())
                config.Images = input.Images!;

            await _configuration.SaveConfigurationAsync(AppConfig.PostSettings, config!);
            return Success();
        }

        /// <summary>
        /// 获取邮件服务设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Email")]
        [Produces("application/json")]
        public async Task<DataOutput<EmailSettingsConfiguration>> EmailSettings()
        {
            var dto = await _configuration.GetConfigurationAsync<EmailSettingsConfiguration>(AppConfig.EmailSettings);
            return Success(dto);
        }

        /// <summary>
        /// 保存邮件服务设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Email")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveEmailSettings([FromBody] SaveEmailSettingsInput input)
        {
            var config = await _configuration.GetConfigurationAsync<EmailSettingsConfiguration>(AppConfig.EmailSettings)!;

            if (!input.EmailAddress.IsNullOrEmpty())
                config!.EmailAddress = input.EmailAddress!;

            if (!input.EmailUser.IsNullOrEmpty())
                config!.EmailUser = input.EmailUser!;

            if (!input.STMPServer.IsNullOrEmpty())
                config!.STMPServer = input.STMPServer!;

            if (input.STMPPort.HasValue)
                config!.STMPPort = input.STMPPort!.Value;

            if (!input.EmailPassword.IsNullOrEmpty())
                config!.EmailPassword = input.EmailPassword!;

            if (!input.EmailSignature.IsNullOrEmpty())
                config!.EmailSignature = input.EmailSignature!;

            if (input.UseSSL.HasValue)
                config!.UseSSL = input.UseSSL!.Value;

            await _configuration.SaveConfigurationAsync(AppConfig.EmailSettings, input.ToMap<EmailSettingsConfiguration>());
            return Success();
        }

        /// <summary>
        /// 获取SEO设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Seo")]
        [Produces("application/json")]
        public async Task<DataOutput<SeoSettingsConfiguration>> SeoSettings()
        {
            var dto = await _configuration.GetConfigurationAsync<SeoSettingsConfiguration>(AppConfig.SeoSettings);
            return Success(dto);
        }

        /// <summary>
        /// 保存SEO设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Seo")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveSeoSettings([FromBody] SaveSeoSettingsInput input)
        {
            var config = await _configuration.GetConfigurationAsync<SeoSettingsConfiguration>(AppConfig.SeoSettings);

            if (input.Biadu != null)
            {
                if (input.Biadu.Enabled.HasValue)
                    config!.Biadu.Enabled = input.Biadu.Enabled.Value;

                if (!input.Biadu.Host.IsNullOrEmpty())
                    config!.Biadu.Host = input.Biadu.Host!;

                if (!input.Biadu.Site.IsNullOrEmpty())
                    config!.Biadu.Site = input.Biadu.Site!;

                if (!input.Biadu.Token.IsNullOrEmpty())
                    config!.Biadu.Token = input.Biadu.Token!;
            }

            await _configuration.SaveConfigurationAsync(AppConfig.SeoSettings, input.ToMap<SeoSettingsConfiguration>());
            return Success();
        }

        /// <summary>
        /// 获取文件存储服务设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("StaticFile")]
        [Produces("application/json")]
        public async Task<DataOutput<StaticFileConfiguration>> StaticFileSettings()
        {
            var dto = await _configuration.GetConfigurationAsync<StaticFileConfiguration>(AppConfig.StaticFile);
            return Success(dto);
        }

        /// <summary>
        /// 保存文件存储服务设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("StaticFile")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveStaticFileSettings([FromBody] SaveStaticFileSettingsInput input)
        {
            var config = CheckStaticFileSettings(input);
            await _configuration.SaveConfigurationAsync(AppConfig.StaticFile, config);
            return Success();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("FileUpload/Channel")]
        [Produces("application/json")]
        public Task<ListOutput<EnumsViewModel<int>>> FileUploadChannelEnum()
        {
            var dto = _configuration.GetFileSaveChannelEnum();
            return Task.FromResult(Success(dto.ToMapList<EnumsViewModel<int>>()));
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<string>> Upload([FromForm] ConfigurationUploadInput input)
        {
            var fileUrl = "";

            if (input.ConfigName == AppConfig.PostSettings)
                fileUrl = await _fileManage.Value.UploadFileAsync(input.File!, "/post");

            return Success(fileUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="HttpStatusException"></exception>
        private static StaticFileConfiguration CheckStaticFileSettings(SaveStaticFileSettingsInput input)
        {
            try
            {
                var channel = (FileUploadChannelEnum)input.UploadChannel!.Value;

                switch (channel)
                {
                    case Model.Configurations.FileUploadChannelEnum.Github:
                        if (input.Github == null)
                            throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, $"github configuration can not convert to null");
                        break;
                    case Model.Configurations.FileUploadChannelEnum.Minio:
                        if (input.Minio == null)
                            throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, $"monio configuration can not convert to null");
                        break;
                    case Model.Configurations.FileUploadChannelEnum.OSS:
                        break;
                    case Model.Configurations.FileUploadChannelEnum.COS:
                        break;
                    case Model.Configurations.FileUploadChannelEnum.OBS:
                        break;
                    case Model.Configurations.FileUploadChannelEnum.Kodo:
                        break;
                    default:
                        break;
                }

                return input.ToMap<StaticFileConfiguration>();
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, $"{nameof(input.UploadChannel)}:{input.UploadChannel} check failed:{ex.Message}");
            }
        }
    }
}
