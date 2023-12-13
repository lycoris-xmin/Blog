using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Configurations;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Email;
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
    /// 系统设置
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
        public async Task<DataOutput<WebSettingsConfiguration>> WebSetting()
        {
            var dto = await _configuration.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSetting);
            return Success(dto);
        }

        /// <summary>
        /// 保存网站基础配置
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("Web")]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<WebSettingsConfiguration>> SaveWebSetting([FromForm] SaveWebSettingInput input, [FromServices] IFileManageAppService fileManage)
        {
            var config = await _configuration.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSetting);

            if (!input.WebName.IsNullOrEmpty())
                config!.WebName = input.WebName!;
            if (!input.WebPath.IsNullOrEmpty())
                config!.WebPath = input.WebPath!;
            if (!input.AdminPath.IsNullOrEmpty())
                config!.AdminPath = input.AdminPath!;
            if (!input.ICP.IsNullOrEmpty())
                config!.ICP = input.ICP!;
            if (!input.Description.IsNullOrEmpty())
                config!.Description = input.Description!;
            if (input.BuildTime.HasValue)
                config!.BuildTime = input.BuildTime!.Value;

            if (input.Logo != null)
                (config!.Logo, _) = await fileManage.UploadFileAsync(input.Logo, StaticsFilePath.Logo);
            else if (input.LogoDisplay.IsNullOrEmpty())
                config!.Logo = "";

            if (input.Avatar != null)
                (config!.DefaultAvatar, _) = await fileManage.UploadFileAsync(input.Avatar, StaticsFilePath.Avatar);

            await _configuration.SaveConfigurationAsync(AppConfig.WebSetting, config!);
            return Success(config);
        }

        /// <summary>
        /// 获取文章设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Post")]
        [Produces("application/json")]
        public async Task<DataOutput<PostSettingConfiguration>> PostSetting()
        {
            var dto = await _configuration.GetConfigurationAsync<PostSettingConfiguration>(AppConfig.PostSetting);
            return Success(dto);
        }

        /// <summary>
        /// 保存文章设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Post")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SavePostSetting([FromBody] SavePostSettingInput input)
        {
            var config = await _configuration.GetConfigurationAsync<PostSettingConfiguration>(AppConfig.PostSetting)!;

            config!.AutoSave = input.AutoSave!.Value;

            if (input.Second.HasValue && input.Second > 0)
                config.Second = input.Second!.Value;

            config.Images = input.Images ?? new List<string>();

            if (input.CommentFrequencySecond.HasValue)
                config.CommentFrequencySecond = input.CommentFrequencySecond!.Value;

            await _configuration.SaveConfigurationAsync(AppConfig.PostSetting, config!);
            return Success();
        }

        /// <summary>
        /// 获取留言设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Message")]
        [Produces("application/json")]
        public async Task<DataOutput<MessageSettingConfiguration>> MessageSetting()
        {
            var dto = await _configuration.GetConfigurationAsync<MessageSettingConfiguration>(AppConfig.MessageSetting);
            return Success(dto);
        }

        /// <summary>
        /// 保存留言设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Message")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> MessageSetting([FromBody] SaveMessageSettingInput input)
        {
            if (input.MessageRemind.HasValue() || input.FrequencySecond.HasValue)
            {
                var config = await _configuration.GetConfigurationAsync<MessageSettingConfiguration>(AppConfig.MessageSetting)!;

                if (input.MessageRemind.HasValue())
                    config!.MessageRemind = input.MessageRemind!;

                if (input.FrequencySecond.HasValue)
                    config!.FrequencySecond = input.FrequencySecond!.Value;

                await _configuration.SaveConfigurationAsync(AppConfig.MessageSetting, config!);
            }

            return Success();
        }

        /// <summary>
        /// 获取邮件服务设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Email")]
        [Produces("application/json")]
        public async Task<DataOutput<EmailSettingsConfiguration>> EmailSetting()
        {
            var dto = await _configuration.GetConfigurationAsync<EmailSettingsConfiguration>(AppConfig.EmailSetting);
            return Success(dto);
        }

        /// <summary>
        /// 保存邮件服务设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Email")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveEmailSetting([FromBody] SaveEmailSettingInput input)
        {
            var config = await _configuration.GetConfigurationAsync<EmailSettingsConfiguration>(AppConfig.EmailSetting)!;

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

            await _configuration.SaveConfigurationAsync(AppConfig.EmailSetting, input.ToMap<EmailSettingsConfiguration>());
            return Success();
        }

        /// <summary>
        /// 发送测试邮件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Email/Test")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> EmailServiceTest([FromBody] EmailServiceTestInput input, [FromServices] IEmailAppService email)
        {
            var option = input.ToMap<EmailSettingsConfiguration>();
            await email.SendTestEmailAsync(input.TestEmail!, option);
            return Success();
        }

        /// <summary>
        /// 获取SEO设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Seo")]
        [Produces("application/json")]
        public async Task<DataOutput<SeoSettingsConfiguration>> SeoSetting()
        {
            var dto = await _configuration.GetConfigurationAsync<SeoSettingsConfiguration>(AppConfig.SeoSetting);
            return Success(dto);
        }

        /// <summary>
        /// 保存SEO设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Seo")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveSeoSetting([FromBody] SaveSeoSettingInput input)
        {
            var config = await _configuration.GetConfigurationAsync<SeoSettingsConfiguration>(AppConfig.SeoSetting);

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

            await _configuration.SaveConfigurationAsync(AppConfig.SeoSetting, input.ToMap<SeoSettingsConfiguration>());
            return Success();
        }

        /// <summary>
        /// 获取文件存储服务设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Upload")]
        [Produces("application/json")]
        public async Task<DataOutput<UploadConfiguration>> UploadSetting()
        {
            var dto = await _configuration.GetConfigurationAsync<UploadConfiguration>(AppConfig.Upload);
            return Success(dto);
        }

        /// <summary>
        /// 保存文件存储服务设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("Upload")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveStaticFileSetting([FromBody] SaveUploadSettingInput input)
        {
            var config = CheckStaticFileSetting(input);
            await _configuration.SaveConfigurationAsync(AppConfig.Upload, config);
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
            var dto = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSetting);
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
            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSetting);
            if (config!.ShowDocHost != input.Host)
            {
                config.ShowDocHost = input.Host!;
                await _configuration.SaveConfigurationAsync(AppConfig.SystemSetting, config);
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

            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSetting);

            if (input.StaticFile!.Value != config!.SystemFileClear.StaticFile || input.TempFile!.Value != config!.SystemFileClear.TempFile || input.LogFile!.Value != config!.SystemFileClear.LogFile)
            {
                config!.SystemFileClear = input.ToMap<SystemFileClearConfiguration>();
                await _configuration.SaveConfigurationAsync(AppConfig.SystemSetting, config);
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

            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSetting);

            if (input.RequestLog!.Value != config!.SystemDBClear.RequestLog || input.BrowseLog!.Value != config!.SystemDBClear.BrowseLog || input.PostComment!.Value != config!.SystemDBClear.PostComment || input.LeaveMessage!.Value != config!.SystemDBClear.LeaveMessage)
            {
                config!.SystemDBClear = input.ToMap<SystemDBClearConfiguration>();
                await _configuration.SaveConfigurationAsync(AppConfig.SystemSetting, config);
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
        private static UploadConfiguration CheckStaticFileSetting(SaveUploadSettingInput input)
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

                return input.ToMap<UploadConfiguration>();
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
