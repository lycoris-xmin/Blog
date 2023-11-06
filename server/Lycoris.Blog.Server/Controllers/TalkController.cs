using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Talks;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Talks;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Talk")]
    public class TalkController : BaseController
    {
        private readonly ITalkAppService _talk;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="talk"></param>
        public TalkController(ITalkAppService talk)
        {
            _talk = talk;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 说说列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<TalkDataViewModel>> TalkList([FromQuery] PageInput input)
        {
            var dto = await _talk.GetTalkListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<TalkDataViewModel>());
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 查询说说列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<MasterTalkDataViewModel>> List([FromQuery] PageInput input)
        {
            var dto = await _talk.GetListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<MasterTalkDataViewModel>());
        }

        /// <summary>
        /// 添加、修改说说
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CreateOrUpdate")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<MasterTalkDataViewModel>> CreateOrUpdate([FromBody] MasterTalkCreateOrUpdateInput input)
        {
            if (input.Content!.Length > 300)
                throw new FriendlyException("");

            var dto = await _talk.CreateOrUpdateAsync(input.Content!, input.Id ?? 0);

            return Success(dto.ToMap<MasterTalkDataViewModel>());
        }

        /// <summary>
        /// 删除说说
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromQuery] long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "");

            await _talk.DeletaAsync(id!.Value);
            return Success();
        }
        #endregion
    }
}

