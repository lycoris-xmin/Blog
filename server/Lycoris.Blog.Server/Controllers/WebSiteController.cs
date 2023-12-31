﻿using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.WebSite;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.WebSite;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 关于
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/WebSite"), AppAuthentication]
    public class WebSiteController : BaseController
    {
        private readonly IWebSiteAppService _webSiteAbout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSiteAbout"></param>
        public WebSiteController(IWebSiteAppService webSiteAbout)
        {
            _webSiteAbout = webSiteAbout;
        }

        /// <summary>
        /// 获取关于本站文章
        /// </summary>
        /// <returns></returns>
        [HttpGet("About/Web")]
        [Produces("application/json")]
        public async Task<DataOutput<string>> WebAbout()
        {
            var dto = await _webSiteAbout.GetAboutAsync(AppAbout.AboutWeb);
            return Success(dto);
        }

        /// <summary>
        /// 保存关于本站信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("About/Web")]
        [GanssXssSettings("Value")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SaveAbout([FromBody] SaveAboutWebInput input)
        {
            await _webSiteAbout.SaveAboutAsync(AppAbout.AboutWeb, input.Value!);
            return Success();
        }

        /// <summary>
        /// 获取关于我相关信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("About/Me/{type}")]
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
                value = await _webSiteAbout.GetAboutAsync(AppAbout.AboutMeInfo);
            else if (type == "skill")
                value = await _webSiteAbout.GetAboutAsync(AppAbout.AboutMeSkill);
            else if (type == "project")
                value = await _webSiteAbout.GetAboutAsync(AppAbout.AboutMeProject);
            else
                value = await _webSiteAbout.GetAboutAsync(AppAbout.AboutMeOffice);

            return Success(value ?? "");
        }

        /// <summary>
        /// 保存关于我相关信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("About/Me")]
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

                await _webSiteAbout.SaveAboutAsync(AppAbout.AboutMeInfo, value);
            }
            else if (input.Type == "skill")
            {
                var value = input.Config!.ToObject<AboutMeSkillConfiguration>() ?? new AboutMeSkillConfiguration();
                value.Description = value.Description?.Where(x => !x.IsNullOrEmpty()).ToList() ?? new List<string>();
                await _webSiteAbout.SaveAboutAsync(AppAbout.AboutMeSkill, value);
            }
            else if (input.Type == "project")
            {
                var value = input.Config!.ToObject<List<AboutMeProjectConfiguration>>();
                await _webSiteAbout.SaveAboutAsync(AppAbout.AboutMeProject, value!);
            }
            else
            {
                var value = input.Config!.ToObject<List<AboutMeOfficeConfiguration>>();
                await _webSiteAbout.SaveAboutAsync(AppAbout.AboutMeOffice, value!);
            }

            return Success();
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("About/Upload")]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<string>> Upload([FromForm] WebSiteFileUploadInput input, [FromServices] IFileManageAppService fileManage)
        {
            var fileUrl = await fileManage.UploadFileAsync(input.File!, input.Path!);
            return Success(fileUrl);
        }
    }
}
