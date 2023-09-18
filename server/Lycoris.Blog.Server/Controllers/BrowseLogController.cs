using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppService.BrowseLogs;
using Lycoris.Blog.Application.AppService.BrowseLogs.Dtos;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.BrowseLogs;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 浏览日志
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/BrowseLog"), AppAuthentication]
    public class BrowseLogController : BaseController
    {
        private readonly IBrowseLogAppService _browseLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browseLog"></param>
        public BrowseLogController(IBrowseLogAppService browseLog)
        {
            _browseLog = browseLog;
        }

        /// <summary>
        /// 查询浏览日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [Produces("application/json")]
        public async Task<PageOutput<BrowseLogDataViewModel>> List([FromQuery] BrowseLogListInput input)
        {
            var filter = input.ToMap<BrowseLogListFilter>();
            var dto = await _browseLog.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<BrowseLogDataViewModel>());
        }

        /// <summary>
        /// 删除浏览日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromBody] SingleIdArrayInput<long> input)
        {
            await _browseLog.DeleteAsync(input.Ids!);
            return Success();
        }

        /// <summary>
        /// 查询跳转来源列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Referer/Query/List")]
        [Produces("application/json")]
        public async Task<PageOutput<BrowseRefererDataViewModel>> RefererList([FromQuery] PageInput input)
        {
            var dto = await _browseLog.GetRefererListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<BrowseRefererDataViewModel>());
        }

        /// <summary>
        /// 删除跳转来源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Referer/Delete")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> DeleteReferer([FromBody] SingleIdArrayInput<long> input)
        {
            await _browseLog.DeleteRefererAsync(input.Ids!);
            return Success();
        }
    }
}
