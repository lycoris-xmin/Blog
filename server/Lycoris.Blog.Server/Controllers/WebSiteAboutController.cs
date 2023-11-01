using Lycoris.Blog.Application.AppServices.WebSiteAbouts;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Configurations;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/WebSite/About"), AppAuthentication]
    public class WebSiteAboutController : BaseController
    {
        private readonly IWebSiteAboutAppService _webSiteAbout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSiteAbout"></param>
        public WebSiteAboutController(IWebSiteAboutAppService webSiteAbout)
        {
            _webSiteAbout = webSiteAbout;
        }

        /// <summary>
        /// 获取关于本站文章
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web")]
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
        [HttpPost("Web")]
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
        [HttpGet("Me/{type}")]
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
        [HttpPost("Me")]
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
    }
}
