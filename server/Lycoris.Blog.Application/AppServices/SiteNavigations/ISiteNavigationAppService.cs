using Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.SiteNavigations
{
    public interface ISiteNavigationAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<EnumsDto<int>>> GetSiteNavigationGroupListAsync();

        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SiteNavigationDataDto>> GetSiteNavigationListAsync(int groupId);
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<SiteNavigationQueryDataDto>> GetListAsync(SiteNavigationQueryFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SiteNavigationQueryDataDto> CreateAsync(CreateSiteNavigationDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SiteNavigationQueryDataDto> UpdateAsync(UpdateSiteNavigationDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);
        #endregion
    }
}
