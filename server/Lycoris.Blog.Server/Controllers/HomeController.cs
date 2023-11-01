using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Home;
using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Models.Home;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
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
        /// 关于本站
        /// </summary>
        /// <returns></returns>
        [HttpGet("About/Web")]
        [Produces("application/json")]
        public async Task<DataOutput<string>> AboutWeb() => Success(await _home.GetAboutWebAsync() ?? "");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web/Owner")]
        [Produces("application/json")]
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
        [Produces("application/json")]
        public async Task<BaseOutput> WebBrowseRecord([FromBody] WebBrowseRecordInput input)
        {
            var data = input.ToMap<WebBrowseRecordDto>();
            await _home.WebBrowseRecordAsync(data);
            return Success();
        }

        /// <summary>
        /// 网站相关统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web/Statistics")]
        [Produces("application/json")]
        public async Task<DataOutput<WebStatisticsViewModel>> WebStatistics()
        {
            var dto = await _home.GetWebStatisticsAsync();
            return Success(dto.ToMap<WebStatisticsViewModel>());
        }

        /// <summary>
        /// 文章相关统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Post/Statistics")]
        [Produces("application/json")]
        public async Task<ListOutput<PostStatisticsViewModel>> PostStatistics()
        {
            // 疑问
            var dto = await _home.GetPostStatisticsAsync();
            return Success(dto.ToMapList<PostStatisticsViewModel>());
        }
    }
}
