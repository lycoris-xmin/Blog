﻿using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.Configurations;
using Lycoris.Blog.Core.CloudStorage.Minio;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Configurations;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Configuration"), AppAuthentication]
    public class ConfigurationController : BaseController
    {
        private readonly IConfigurationAppService _configuration;
        private readonly Lazy<IMinioService> _minio;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="minio"></param>
        public ConfigurationController(IConfigurationAppService configuration, Lazy<IMinioService> minio)
        {
            _configuration = configuration;
            _minio = minio;
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
        [HttpGet("FileUpload")]
        [Produces("application/json")]
        public async Task<DataOutput<FileUploadConfiguration>> FileUploadSettings()
        {
            var dto = await _configuration.GetConfigurationAsync<FileUploadConfiguration>(AppConfig.FileUpload);
            return Success(dto);
        }

        /// <summary>
        /// 保存文件存储服务设置
        /// </summary>
        /// <returns></returns>
        [HttpPost("FileUpload")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveFileUploadSettings([FromBody] SaveFileUploadSettingsInput input)
        {
            await _configuration.SaveConfigurationAsync(AppConfig.FileUpload, input.ToMap<FileUploadConfiguration>());
            return Success();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("FileUpload/Channel")]
        [Produces("application/json")]
        public Task<ListOutput<EnumsViewModel<int>>> SaveFileChannelEnum()
        {
            var dto = _configuration.GetSaveChannelEnum();
            return Task.FromResult(Success(dto.ToMapList<EnumsViewModel<int>>()));
        }

        /// <summary>
        /// 获取关于本站文章
        /// </summary>
        /// <returns></returns>
        [HttpGet("AboutWeb")]
        [Produces("application/json")]
        public async Task<DataOutput<string>> About()
        {
            var dto = await _configuration.GetConfigurationAsync(AppConfig.AboutWeb);
            return Success(dto);
        }

        /// <summary>
        /// 保存关于本站信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("AboutWeb")]
        [GanssXssSettings("Value")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveAbout([FromBody] SaveAboutWebInput input)
        {
            await _configuration.SaveConfigurationAsync(AppConfig.AboutWeb, input.Value!);
            return Success();
        }

        /// <summary>
        /// 获取关于我相关信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("AboutMe/{type}")]
        [Produces("application/json")]
        public async Task<DataOutput<string>> AboutMe(string? type)
        {
            if (type.IsNullOrEmpty())
                throw new HttpStatusException(HttpStatusCode.NotFound);

            type = type!.ToLower();

            if (!new string[] { "info", "skill", "project", "office" }.Contains(type))
                throw new HttpStatusException(HttpStatusCode.NotFound);

            string? value;
            if (type == "info")
                value = await _configuration.GetConfigurationAsync(AppConfig.AboutMeInfo);
            else if (type == "skill")
                value = await _configuration.GetConfigurationAsync(AppConfig.AboutMeSkill);
            else if (type == "project")
                value = await _configuration.GetConfigurationAsync(AppConfig.AboutMeProject);
            else
                value = await _configuration.GetConfigurationAsync(AppConfig.AboutMeOffice);

            return Success(value ?? "");
        }

        /// <summary>
        /// 保存关于我相关信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AboutMe")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveAboutMe([FromBody] SaveAboutMeInput input)
        {
            input.Type = input.Type!.ToLower();

            if (!new string[] { "info", "skill", "project", "office" }.Contains(input.Type))
                throw new HttpStatusException(HttpStatusCode.NotFound, "");

            if (input.Type == "info")
            {
                var value = input.Config!.ToObject<AboutMeInfoConfiguration>() ?? new AboutMeInfoConfiguration();

                value.Description = value.Description?.Where(x => !x.IsNullOrEmpty()).ToList() ?? new List<string>();
                value.Code = value.Code?.Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                value.Hobby = value.Hobby?.Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                value.Introduction = value.Introduction?.Where(x => !x.IsNullOrEmpty()).ToList() ?? new List<string>();

                await _configuration.SaveConfigurationAsync(AppConfig.AboutMeInfo, value);
            }
            else if (input.Type == "skill")
            {
                var value = input.Config!.ToObject<AboutMeSkillConfiguration>() ?? new AboutMeSkillConfiguration();
                value.Description = value.Description?.Where(x => !x.IsNullOrEmpty()).ToList() ?? new List<string>();
                await _configuration.SaveConfigurationAsync(AppConfig.AboutMeSkill, value);
            }
            else if (input.Type == "project")
            {
                var value = input.Config!.ToObject<List<AboutMeProjectConfiguration>>();
                await _configuration.SaveConfigurationAsync(AppConfig.AboutMeProject, value!);
            }
            else
            {
                var value = input.Config!.ToObject<List<AboutMeOfficeConfiguration>>();
                await _configuration.SaveConfigurationAsync(AppConfig.AboutMeOffice, value!);
            }

            return Success();
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<string>> UploadAsync([FromForm] ConfigurationUploadInput input)
        {
            var fileUrl = "";

            if (input.ConfigName == AppConfig.PostSettings)
            {
                fileUrl = await _minio.Value.UploadFileAsync(x =>
                {
                    x.WithBucketPath("/post");
                    x.WithFormFile(input.File!);
                });
            }
            else if (input.ConfigName == AppConfig.AboutWeb)
            {
                fileUrl = await _minio.Value.UploadFileAsync(x =>
                {
                    x.WithBucketPath($"/about");
                    x.WithFormFile(input.File!);
                });
            }

            return Success(fileUrl);
        }
    }
}
