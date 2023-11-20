using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.Users;
using Lycoris.Blog.Application.AppServices.Users.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Common;
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

        #region 博客网站

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
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("Brief/Update")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<UserBriefViewModel>> UpdateUserBrief([FromForm] UpdateUserBriefInput input, [FromServices] Lazy<IFileManageAppService> fileManage)
        {
            var data = input.ToMap<UserBriefDto>();
            if (input.File != null)
                data.Avatar = await fileManage.Value.UploadFileAsync(input.File, StaticsFilePath.Avatar);

            var dto = await _user.UpdateUserBrieAsync(data);

            return Success(dto.ToMap<UserBriefViewModel>());
        }

        #endregion

        #region 管理后台

        /// <summary>
        /// 用户简要信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Dashboard/Brief")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<DataOutput<UserBriefViewModel>> DashboardUserBrief()
        {
            var dto = await _user.GetUserBriefAsync();
            return Success(dto?.ToMap<UserBriefViewModel>() ?? new UserBriefViewModel());
        }

        /// <summary>
        /// 更新用户简要信息
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("Dashboard/Brief/Update")]
        [AppAuthentication]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<UserBriefViewModel>> UpdateDashboardUserBrief([FromForm] UpdateUserBriefInput input, [FromServices] Lazy<IFileManageAppService> fileManage)
        {
            var data = input.ToMap<UserBriefDto>();
            if (input.File != null)
                data.Avatar = await fileManage.Value.UploadFileAsync(input.File, StaticsFilePath.Avatar);

            var dto = await _user.UpdateUserBrieAsync(data);

            return Success(dto.ToMap<UserBriefViewModel>());
        }

        /// <summary>
        /// 用户状态枚举
        /// </summary>
        /// <returns></returns>
        [HttpGet("Status/Enum")]
        [AppAuthentication]
        [Produces("application/json")]
        public Task<ListOutput<EnumsViewModel<int>>> UserStatusEnum()
        {
            var dto = _user.GetUserStatusEnums();
            return Task.FromResult(Success(dto.ToMapList<EnumsViewModel<int>>()));
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<UserDataViewModel>> List([FromQuery] UserListInput input)
        {
            var filter = input.ToMap<GetUserListFilter>();
            var dto = await _user.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<UserDataViewModel>());
        }

        /// <summary>
        /// 获取用户绑定信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Link")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<DataOutput<UserLinkViewModel>> UserLink([FromQuery] SingleIdInput<long?> input)
        {
            var dto = await _user.GetUserLinkAsync(input.Id!.Value);
            return Success(dto.ToMap<UserLinkViewModel>());
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<UserDataViewModel>> Create([FromBody] UserCreateInput input)
        {
            var data = input.ToMap<CreateUserDto>();
            var dto = await _user.CreateUserAsync(data);
            return Success(dto.ToMap<UserDataViewModel>());
        }

        /// <summary>
        /// 审核用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Audit")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Audit([FromBody] UserAuditInput input)
        {
            var data = input.ToMap<AuditUserDto>();
            await _user.AuditUserAsync(data);
            return Success();
        }
        #endregion
    }
}
