using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FriendLinks;
using Lycoris.Blog.Application.AppServices.FriendLinks.Dtos;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Application.Swaggers;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.FriendLinks;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/FriendLink")]
    public class FriendLinkController : BaseController
    {
        private readonly IFriendLinkAppService _friendLink;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendLink"></param>
        public FriendLinkController(IFriendLinkAppService friendLink)
        {
            _friendLink = friendLink;
        }


        #region ======== 博客网站 ========
        /// <summary>
        /// 博客网站获取友情链接列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [ExcludeSwaggerHeader, Produces("application/json")]
        public async Task<ListOutput<FriendLinkDataViewModel>> FriendLinkList([FromQuery] PageInput input)
        {
            var dto = await _friendLink.GetFriendLinkListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.ToMapList<FriendLinkDataViewModel>());
        }

        /// <summary>
        /// 友情链接申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Apply")]
        [WebAuthentication(IsRequired = true)]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> FriendLinkApply([FromBody] FriendLinkApplyInput input)
        {
            if (!input.Description.IsNullOrEmpty() && input.Description!.Length > 100)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var data = input.ToMap<FriendLinkApplyDto>();
            await _friendLink.FriendLinkApplyAsync(data);
            return Success();
        }

        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 查询友情链接列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<FriendLinkQueryDataViewModel>> List([FromQuery] FriendLinkQueryListInput input)
        {
            var filter = input.ToMap<FriendLinkQueryFilter>();
            var dto = await _friendLink.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<FriendLinkQueryDataViewModel>());
        }

        /// <summary>
        /// 创建友情链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<FriendLinkQueryDataViewModel>> Create([FromBody] FriendLinkCreateInput input)
        {
            var data = input.ToMap<CreateFriendLinkDto>();
            var dto = await _friendLink.CreateAsync(data);
            return Success(dto.ToMap<FriendLinkQueryDataViewModel>());
        }

        /// <summary>
        /// 审核友情链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Audit")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<string>> Audit([FromBody] FriendLinkAudtInput input)
        {
            var data = input.ToMap<AuditFriendLinkDto>();
            var res = await _friendLink.AuditAsync(data);
            return Success(res);
        }

        /// <summary>
        /// 删除友情链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> DeleteAsync([FromBody] SingleIdInput<int?> input)
        {
            await _friendLink.DeleteAsync(input.Id!.Value);
            return Success();
        }
        #endregion
    }
}
