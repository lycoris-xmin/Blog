using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppService.Users;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Models.Users;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/User")]
    public class UserController : BaseController
    {
        private readonly IUserAppService _user;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public UserController(IUserAppService user)
        {
            _user = user;
        }

        /// <summary>
        /// 用户简要信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Brief")]
        [WebAuthentication(IsRequired = true)]
        [Produces("application/json")]
        public async Task<DataOutput<UserBriefViewModel>> UserBrief()
        {
            var dto = await _user.GetUserBriefAsync();
            return Success(dto?.ToMap<UserBriefViewModel>() ?? new UserBriefViewModel());
        }

        /// <summary>
        /// 用户简要信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Brief/{id}")]
        [Produces("application/json")]
        public async Task<DataOutput<UserBriefViewModel>> UserBrief(long id)
        {
            if (id <= 0)
                throw new HttpStatusException(System.Net.HttpStatusCode.NoContent);

            var dto = await _user.GetUserBriefAsync(id);
            return Success(dto?.ToMap<UserBriefViewModel>() ?? new UserBriefViewModel());
        }

        /// <summary>
        /// 更新用户简要信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Brief/Update")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<BaseOutput> UpdateUserBrief([FromForm] UpdateUserBriefInput input)
        {
            var data = input.ToMap<UserBriefDto>();
            await _user.UpdateUserBrieAsync(data, input.File);
            return Success();
        }
    }
}
