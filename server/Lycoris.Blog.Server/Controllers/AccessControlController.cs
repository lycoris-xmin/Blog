using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.AccessControls;
using Lycoris.Blog.Application.AppServices.AccessControls.Dtos;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.AccessControls;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 访问管控
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/AccessControl")]
    [AppAuthentication]
    public class AccessControlController : BaseApiController
    {
        private readonly IAccessControlAppService _accessControl;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessControl"></param>
        public AccessControlController(IAccessControlAppService accessControl)
        {
            _accessControl = accessControl;
        }

        /// <summary>
        /// 获取访问管控列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<AccessControlDataViewModel>> List([FromQuery] AccessControlListInput input)
        {
            var filter = input.ToMap<GetAccessControlListFilter>();
            var dto = await _accessControl.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<AccessControlDataViewModel>());
        }

        /// <summary>
        /// 添加访问管控
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<AccessControlDataViewModel>> Create([FromBody] AccessControlCreateInput input)
        {
            var dto = await _accessControl.CreateAsync(input.Ip!);
            return Success(dto.ToMap<AccessControlDataViewModel>());
        }

        /// <summary>
        /// 移除管控
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromBody] SingleIdInput<int?> input)
        {
            await _accessControl.DeleteAsync(input.Id!.Value);
            return Success();
        }

        /// <summary>
        /// 获取管控的访问日志列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Log/List")]
        [Produces("application/json")]
        public async Task<PageOutput<AccessControlLogDataViewModel>> AccessControlLogList([FromQuery] AccessControlLogListInput input)
        {
            var filter = input.ToMap<GetAccessControlLogListFilter>();
            var dto = await _accessControl.GetAccessControlLogListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<AccessControlLogDataViewModel>());
        }
    }
}
