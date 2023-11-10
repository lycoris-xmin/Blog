using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.AccessControls;
using Lycoris.Blog.Application.AppServices.AccessControls.Dtos;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Models.AccessControls;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/AccessControl")]
    public class AccessControlController : BaseController
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
    }
}
