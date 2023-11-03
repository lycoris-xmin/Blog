using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.SiteNavigations;
using Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Models.SiteNavigations;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/SiteNavigation")]
    public class SiteNavigationController : BaseController
    {
        private readonly ISiteNavigationAppService _siteNavigation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteNavigation"></param>
        public SiteNavigationController(ISiteNavigationAppService siteNavigation)
        {
            _siteNavigation = siteNavigation;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 网站收录列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("List")]
        [ Produces("application/json")]
        public async Task<ListOutput<SiteNavigationDataViewModel>> SiteNavigationList()
        {
            var dto = await _siteNavigation.GetSiteNavigationListAsync();
            return Success(dto.ToMapList<SiteNavigationDataViewModel>());
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 查询网站收录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Query/List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<SiteNavigationQueryDataViewModel>> List([FromQuery] SiteNavigationQueryListInput input)
        {
            var filter = input.ToMap<SiteNavigationQueryFilter>();
            var dto = await _siteNavigation.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<SiteNavigationQueryDataViewModel>());
        }

        /// <summary>
        /// 添加网站收录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<SiteNavigationQueryDataViewModel>> Create([FromBody] SiteNavigationCreateInput input)
        {
            var data = input.ToMap<CreateSiteNavigationDto>();
            var dto = await _siteNavigation.CreateAsync(data);
            return Success(dto.ToMap<SiteNavigationQueryDataViewModel>());
        }

        /// <summary>
        /// 修改网站收录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<DataOutput<SiteNavigationQueryDataViewModel>> Update([FromBody] SiteNavigationUpdateInput input)
        {
            var data = input.ToMap<UpdateSiteNavigationDto>();
            var dto = await _siteNavigation.UpdateAsync(data);
            return Success(dto.ToMap<SiteNavigationQueryDataViewModel>());
        }

        /// <summary>
        /// 删除网站收录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromBody] SingleIdInput<int?> input)
        {
            await _siteNavigation.DeleteAsync(input.Id!.Value);
            return Success();
        }

        /// <summary>
        /// 网站收录分组枚举
        /// </summary>
        /// <returns></returns>
        [HttpGet("Enum/Group")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<ListOutput<EnumsViewModel<string>>> GroupEnum()
        {
            var dto = await _siteNavigation.GetSiteNavigationGroupAsync();
            return Success(dto.ToMapList<EnumsViewModel<string>>());
        }
        #endregion
    }
}
