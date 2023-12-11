using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Home;
using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Home;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 网站首页
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Home")]
    public class HomeController : BaseController
    {
        private readonly IHomeAppService _home;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="home"></param>
        public HomeController(IHomeAppService home)
        {
            _home = home;
        }

        /// <summary>
        /// 网站设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web/Setting")]
        [Produces("application/json")]
        [ResponseCache(Duration = 60000, Location = ResponseCacheLocation.Client)]
        public async Task<DataOutput<WebSettingViewModel>> WebSetting()
        {
            var dto = await _home.GetWebSettingsAsync();
            return Success(dto.ToMap<WebSettingViewModel>());
        }

        /// <summary>
        /// 关于本站
        /// </summary>
        /// <returns></returns>
        [HttpGet("About/Web")]
        [Produces("application/json")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<DataOutput<string>> AboutWeb() => Success(await _home.GetAboutWebAsync() ?? "");

        /// <summary>
        /// 站长信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web/Owner")]
        [Produces("application/json")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<DataOutput<WebOwnerViewModel>> WebOnwer()
        {
            var dto = await _home.GetWebOwnerAsync();
            return Success(dto.ToMap<WebOwnerViewModel>());
        }

        /// <summary>
        /// 关于我
        /// </summary>
        /// <returns></returns>
        [HttpGet("About/Me")]
        [Produces("application/json")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<DataOutput<AboutMeViewModel>> AboutMe()
        {
            var res = new AboutMeViewModel();

            var info = await _home.GetAboutMeAsync<AboutMeInfoConfiguration>(AppAbout.AboutMeInfo);
            if (info != null)
                res.Info = info.ToMap<AboutMeInfoViewModel>();

            res.Skill = await _home.GetAboutMeAsync<AboutMeSkillConfiguration>(AppAbout.AboutMeSkill);

            return Success(res);
        }

        /// <summary>
        /// 浏览日志记录
        /// </summary>
        /// <returns></returns>
        [HttpPost("Web/Browse/Record")]
        [WebAuthentication]
        [Produces("application/json")]
        public async Task<BaseOutput> WebBrowseRecord([FromBody] WebBrowseRecordInput input)
        {
            var data = input.ToMap<WebBrowseRecordDto>();
            await _home.WebBrowseRecordAsync(data);
            return Success();
        }

        /// <summary>
        /// 获取创建的信息统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("Publish/Statistics")]
        [Produces("application/json")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<DataOutput<PublishStatisticsViewModel>> PublishStatistics()
        {
            var dto = await _home.GetPublishStatisticsAsync();
            return Success(dto.ToMap<PublishStatisticsViewModel>());
        }

        /// <summary>
        /// 网站相关统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Interactive/Statistics")]
        [Produces("application/json")]
        public async Task<DataOutput<InteractiveStatisticsViewModel>> InteractiveStatistics()
        {
            var dto = await _home.GetInteractiveStatisticsAsync();
            return Success(dto.ToMap<InteractiveStatisticsViewModel>());
        }

        /// <summary>
        /// 文章相关统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Category/Statistics")]
        [Produces("application/json")]
        public async Task<ListOutput<CategoryStatisticsViewModel>> CategoryStatistics()
        {
            // 疑问
            var dto = await _home.GetCategoryStatisticsAsync();
            return Success(dto.ToMapList<CategoryStatisticsViewModel>());
        }

        /// <summary>
        /// 文章随机图
        /// </summary>
        /// <returns></returns>
        [HttpGet("Post/Icon")]
        [Produces("application/json")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<ListOutput<string>> PostIcon()
        {
            var dto = await _home.GetPostIconAsync();
            return Success(dto);
        }
    }
}
