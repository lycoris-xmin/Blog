using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppService.Categorys;
using Lycoris.Blog.Application.AppService.Categorys.Dtos;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Application.Swaggers;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Categorys;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Category")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryAppService _category;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        public CategoryController(ICategoryAppService category)
        {
            _category = category;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 首页文章分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Header")]
        [ExcludeSwaggerHeader, Produces("application/json")]
        public async Task<ListOutput<CategoryHeaderDataViewModel>> CategoryHeaderList()
        {
            var dto = await _category.GetHomeCategoryListAsync();
            return Success(dto.ToMapList<CategoryHeaderDataViewModel>());
        }
        #endregion

        #region ======== 网站后台 ========
        /// <summary>
        /// 查询文章分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("List")]
        [AppAuthentication]
        [Produces("application/json")]
        public async Task<PageOutput<CategoryDataViewModel>> List([FromQuery] PageInput input)
        {
            var dto = await _category.GetListAsync(input.PageIndex!.Value, input.PageSize!.Value);
            return Success(dto.Count, dto.List.ToMapList<CategoryDataViewModel>());
        }

        /// <summary>
        /// 创建文章分类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AppAuthentication]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<CategoryDataViewModel>> Create([FromForm] CategoryCreateInput input)
        {
            var data = input.ToMap<CreateCategoryDto>();
            var dto = await _category.CreateAsync(data);
            return Success(dto.ToMap<CategoryDataViewModel>());
        }

        /// <summary>
        /// 修改文章分类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [AppAuthentication]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<CategoryDataViewModel>> Update([FromForm] CategoryUpdateInput input)
        {
            var data = input.ToMap<UpdateCategoryDto>();
            var dto = await _category.UpdateAsync(data);
            return Success(dto.ToMap<CategoryDataViewModel>());
        }

        /// <summary>
        /// 删除文章分类
        /// </summary>
        /// <returns></returns>
        [HttpPost("Delete")]
        [AppAuthentication]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> Delete([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, $"id:{id} is not valid");

            await _category.DeleteAsync(id.Value);

            return Success();
        }
        #endregion

        #region ======== 网站公用 ========

        /// <summary>
        /// 文章分类枚举
        /// </summary>
        /// <returns></returns>
        [HttpGet("Enum")]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<ListOutput<EnumsViewModel<int>>> Enum()
        {
            var dto = await _category.GetEnumAsync();
            return Success(dto.ToMapList<EnumsViewModel<int>>());
        }

        #endregion
    }
}
