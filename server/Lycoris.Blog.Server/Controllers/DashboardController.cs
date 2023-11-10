using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Dashboard;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Dashboard;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Dashboard"), AppAuthentication]
    public class DashboardController : BaseController
    {
        private readonly IDashboardAppService _dashboard;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashboard"></param>
        public DashboardController(IDashboardAppService dashboard)
        {
            _dashboard = dashboard;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Web/Statistics")]
        [Produces("application/json")]
        public async Task<DataOutput<ServerStatisticsViewModel>> WebStatistics()
        {
            var dto = await _dashboard.GetWebStatisticsAsync();
            return Success(dto.ToMap<ServerStatisticsViewModel>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("NearlyDays/Web/Statistics")]
        [Produces("application/json")]
        public async Task<ListOutput<NearlyDaysWebStatisticsDataViewModel>> NearlyDaysWebStatistics()
        {
            var dto = await _dashboard.GetNearlyDaysWebStatisticsListAsync();
            return Success(dto.ToMapList<NearlyDaysWebStatisticsDataViewModel>());
        }
    }
}
