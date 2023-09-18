using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.Categorys.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.CloudStorage.Minio;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppService.Categorys.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class CategoryAppService : ApplicationBaseService, ICategoryAppService
    {
        private readonly IRepository<Category, int> _category;
        private readonly IMinioService _minio;

        public CategoryAppService(IRepository<Category, int> category, IMinioService minio)
        {
            _category = category;
            _minio = minio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResultDto<CategoryDataDto>> GetListAsync(int pageIndex, int pageSize)
        {
            var filter = _category.GetAll();

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<CategoryDataDto>(count);

            var query = filter.PageBy(pageIndex, pageSize)
                              .Select(x => new CategoryDataDto()
                              {
                                  Id = x.Id,
                                  Name = x.Name,
                                  Keyword = x.Keyword,
                                  Icon = x.Icon,
                                  PostCount = x.PostCount,
                                  CreateTime = x.CreateTime
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<CategoryDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<CategoryDataDto> CreateAsync(CreateCategoryDto input)
        {
            var repeatName = await _category.ExistsAsync(x => x.Name == input.Name);
            if (repeatName)
                throw new FriendlyException("分类名称重复");

            if (input.File != null)
            {
                input.Icon = await _minio.UploadFileAsync(x =>
                {
                    x.WithBucketPath("/category");
                    x.WithFormFile(input.File!);
                });
            }

            var data = input.ToMap<Category>();

            data.Keyword = string.Join(",", data.Keyword.Split(',').Distinct().ToArray());

            data = await _category.CreateAsync(data);

            return data.ToMap<CategoryDataDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CategoryDataDto> UpdateAsync(UpdateCategoryDto input)
        {
            var data = await _category.GetAsync(input.Id) ?? throw new FriendlyException("数据不存在");

            var oldIcon = "";

            if (input.File != null)
            {
                input.Icon = await _minio.UploadFileAsync(x =>
                {
                    x.WithBucketPath("/category");
                    x.WithFormFile(input.File!);
                });

                oldIcon = data.Icon;
            }

            input.Keyword = string.Join(",", input.Keyword?.Split(',')?.Distinct().ToArray() ?? Array.Empty<string>());

            var fieIds = new List<Expression<Func<Category, object>>>();

            data.UpdatePorpertyIf(!input.Name.IsNullOrEmpty() && input.Name != data.Name, x =>
            {
                x.Name = input.Name!;
                fieIds.Add(x => x.Name!);
            }).UpdatePorpertyIf(input.Keyword != null && input.Keyword != data.Keyword, x =>
            {
                data.Keyword = input.Keyword!;
                fieIds.Add(x => x.Keyword!);
            }).UpdatePorpertyIf(!input.Icon.IsNullOrEmpty() && input.Icon != data.Icon, x =>
            {
                data.Icon = input.Icon!;
                fieIds.Add(x => x.Icon!);
            });

            if (fieIds.Any())
                await _category.UpdateFieIdsAsync(data, fieIds);

            if (oldIcon.IsNullOrEmpty())
                await _minio.RemoveFileAsync(x => x.WithFileUrl(oldIcon));

            return data.ToMap<CategoryDataDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var data = await _category.GetAsync(id);
            if (data == null)
                return;

            if (!data.Icon.IsNullOrEmpty())
            {
                // 移除minio里的文件
            }

            await _category.DeleteAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnumsDto<int>>> GetEnumAsync()
        {
            return await _category.GetAll().Select(x => new EnumsDto<int>()
            {
                Value = x.Id,
                Name = x.Name,
                Data = x.Icon
            }).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<HomeCategoryDataDto>> GetHomeCategoryListAsync()
        {
            return await _category.GetAll().Select(x => new HomeCategoryDataDto()
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.PostCount
            }).ToListAsync();
        }
    }
}

