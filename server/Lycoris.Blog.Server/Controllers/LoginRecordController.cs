using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.LoginRecords;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.LoginRecords;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 登录记录
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/LoginRecord")]
    [AppAuthentication]
    public class LoginRecordController : BaseApiController
    {
        private readonly ILoginRecordAppService _loginRecord;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginRecord"></param>
        public LoginRecordController(ILoginRecordAppService loginRecord)
        {
            _loginRecord = loginRecord;
        }

        /// <summary>
        /// 登录记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<LoginRecordDataViewModel>> List([FromQuery] PageInput input)
        {
            var dto = await _loginRecord.GetListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<LoginRecordDataViewModel>());
        }
    }
}
