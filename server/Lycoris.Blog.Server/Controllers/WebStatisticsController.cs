using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.WebStatistics;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.WebStatistics;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/WebStatistics")]
    [AppAuthentication]
    public class WebStatisticsController : BaseController
    {
        private readonly IWebStatisticsAppService _webStatistics;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webStatistics"></param>
        public WebStatisticsController(IWebStatisticsAppService webStatistics)
        {
            _webStatistics = webStatistics;
        }

        /// <summary>
        /// 浏览分布数据统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("WorldMap/List")]
        [Produces("application/json")]
        public async Task<ListOutput<WorldMapDataViewModel>> WorldMapList()
        {
            var dto = await _webStatistics.GetWorldBrowseMapListAsync();
            return Success(dto.ToMapList<WorldMapDataViewModel>());
        }
    }
}
