using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Configurations;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Core.Showdoc;
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
        private readonly Lazy<IShowdocService> _showdoc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileManage"></param>
        /// <param name="showdoc"></param>
        public ConfigurationController(IConfigurationAppService configuration, Lazy<IFileManageAppService> fileManage, Lazy<IShowdocService> showdoc)
        {
            _configuration = configuration;
            _fileManage = fileManage;
            _showdoc = showdoc;
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

            config.Images = input.Images ?? new List<string>();

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
        /// 获取系统设置配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("SystemSettings")]
        [Produces("application/json")]
        public async Task<DataOutput<SystemSettingsConfiguration>> SystemSettingsConfiguration()
        {
            var dto = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSettings);
            return Success(dto);
        }

        /// <summary>
        /// 保存Showdoc公众号推送配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SystemSettings/Showdoc")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveShowdocPushConfiguration([FromBody] SaveShowdocPushConfigurationInput input)
        {
            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSettings);
            if (config!.ShowDocHost != input.Host)
            {
                config.ShowDocHost = input.Host!;
                await _configuration.SaveConfigurationAsync(AppConfig.SystemSettings, config);
            }

            return Success();
        }

        /// <summary>
        /// 保存系统文件清理配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SystemSettings/FileClear")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveSystemFileClearConfiguration([FromBody] SaveSystemFileClearConfigurationInput input)
        {
            if (input.StaticFile!.Value < 1 || input.StaticFile!.Value > 30)
                throw new FriendlyException("保留范围为 1 - 30 天");
            else if (input.TempFile!.Value < 1 || input.TempFile!.Value > 30)
                throw new FriendlyException("保留范围为 1 - 30 天");
            else if (input.LogFile!.Value < 1 || input.LogFile!.Value > 30)
                throw new FriendlyException("保留范围为 1 - 30 天");

            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSettings);

            if (input.StaticFile!.Value != config!.SystemFileClear.StaticFile || input.TempFile!.Value != config!.SystemFileClear.TempFile || input.LogFile!.Value != config!.SystemFileClear.LogFile)
            {
                config!.SystemFileClear = input.ToMap<SystemFileClearConfiguration>();
                await _configuration.SaveConfigurationAsync(AppConfig.SystemSettings, config);
            }

            return Success();
        }

        /// <summary>
        /// 保存数据库清理配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SystemSettings/DBClear")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveSystemDBClearConfiguration([FromBody] SaveSystemDBClearConfigurationInput input)
        {
            if (input.RequestLog!.Value < 0 || input.RequestLog!.Value > 30)
                throw new FriendlyException("保留范围为 0 - 365 天");
            else if (input.BrowseLog!.Value < 0 || input.BrowseLog!.Value > 30)
                throw new FriendlyException("保留范围为 0 - 365 天");
            else if (input.PostComment!.Value < 0 || input.PostComment!.Value > 30)
                throw new FriendlyException("保留范围为 0 - 365 天");
            else if (input.LeaveMessage!.Value < 0 || input.LeaveMessage!.Value > 30)
                throw new FriendlyException("保留范围为 0 - 365 天");

            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSettings);

            if (input.RequestLog!.Value != config!.SystemDBClear.RequestLog || input.BrowseLog!.Value != config!.SystemDBClear.BrowseLog || input.PostComment!.Value != config!.SystemDBClear.PostComment || input.LeaveMessage!.Value != config!.SystemDBClear.LeaveMessage)
            {
                config!.SystemDBClear = input.ToMap<SystemDBClearConfiguration>();
                await _configuration.SaveConfigurationAsync(AppConfig.SystemSettings, config);
            }

            return Success();
        }

        /// <summary>
        /// 静态文件保存渠道枚举
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
                fileUrl = await _fileManage.Value.UploadFileAsync(input.File!, "/post/carousel");

            return Success(fileUrl);
        }

        /// <summary>
        /// Showodc推送测试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Showdoc/PushTest")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> ShowdocPushTest([FromBody] ShowdocPushTestInput input)
        {
            await _showdoc.Value.PublishAsync(input.Host!, input.Title!, input.Content!);
            return Success(input);
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
