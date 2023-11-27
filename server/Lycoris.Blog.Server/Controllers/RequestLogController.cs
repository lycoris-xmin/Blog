using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.RequestLogs;
using Lycoris.Blog.Application.AppServices.RequestLogs.Dtos;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.RequestLogs;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 请求日志
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/RequestLog"), AppAuthentication]
    public class RequestLogController : BaseController
    {
        private readonly IRequestLogAppService _requestLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestLog"></param>
        public RequestLogController(IRequestLogAppService requestLog)
        {
            _requestLog = requestLog;
        }

        /// <summary>
        /// 查询请求日志列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<RequestLogDataViewModel>> List([FromQuery] RequestLogListInput input)
        {
            var filter = input.ToMap<GetRequestLogListFilter>();
            var dto = await _requestLog.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<RequestLogDataViewModel>());
        }

        /// <summary>
        /// 请求日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Info")]
        [Produces("application/json")]
        public async Task<DataOutput<RequestLogInfoViewModel>> Info([FromQuery] long id)
        {
            var dto = await _requestLog.GetInfoAsync(id);
            return Success(dto.ToMap<RequestLogInfoViewModel>());
        }

        /// <summary>
        /// 删除请求日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromBody] SingleIdArrayInput<long> input)
        {
            if (input.Ids.HasValue())
                await _requestLog.DeleteAsync(input.Ids!);

            return Success();
        }

        /// <summary>
        /// 设置访问管控
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AccessControl")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SetAccessControl([FromBody] SetAccessControlInput input)
        {
            await _requestLog.SetAccessControlAsync(input.Ip!);
            return Success();
        }
    }
}
