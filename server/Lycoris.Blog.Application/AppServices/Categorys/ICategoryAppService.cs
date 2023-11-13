using Lycoris.Blog.Application.AppServices.Categorys.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Categorys
{
    public interface ICategoryAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResultDto<CategoryDataDto>> GetListAsync(int pageIndex, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CategoryDataDto> CreateAsync(CreateCategoryDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CategoryDataDto> UpdateAsync(UpdateCategoryDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<EnumsDto<int>>> GetCategoryEnumsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<HomeCategoryDataDto>> GetHomeCategoryListAsync();
    }
}
