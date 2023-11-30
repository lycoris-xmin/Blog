using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.WebStatistics;
using Lycoris.Blog.Model.Global.Input;
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
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Browse/List")]
        [Produces("application/json")]
        public async Task<PageOutput<BrowseStatisticsDataViewModel>> BrowseStatisticsList([FromQuery] BrowseStatisticsListInput input)
        {
            var dto = await _webStatistics.GetBrowseStatisticsListAsync(input.PageIndex!.Value, input.PageSize!.Value, input.Sum ?? false);
            return Success(dto.Count, dto.Summary?.ToMap<BrowseStatisticsDataViewModel>(), dto.List.ToMapList<BrowseStatisticsDataViewModel>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Referer/List")]
        [Produces("application/json")]
        public async Task<PageOutput<RefererStatisticsDataViewModel>> RefererStatisticsList([FromQuery] RefererStatisticsListInput input)
        {
            var dto = await _webStatistics.GetRefererStatisticsListAsync(input.PageIndex!.Value, input.PageSize!.Value, input.Sum ?? false);
            return Success(dto.Count, dto.Summary?.ToMap<RefererStatisticsDataViewModel>(), dto.List.ToMapList<RefererStatisticsDataViewModel>());
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
