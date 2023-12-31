﻿using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.Categorys.Dtos;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppServices.Categorys.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class CategoryAppService : ApplicationBaseService, ICategoryAppService
    {
        private readonly IRepository<Category, int> _category;
        private readonly Lazy<IFileManageAppService> _fileManage;

        public CategoryAppService(IRepository<Category, int> category, Lazy<IFileManageAppService> fileManage)
        {
            _category = category;
            _fileManage = fileManage;
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
                oldIcon = data.Icon;
                data.Icon = input.Icon!;
                fieIds.Add(x => x.Icon!);
            });

            if (fieIds.Any())
                await _category.UpdateFieIdsAsync(data, fieIds);

            if (oldIcon.IsNullOrEmpty())
                await _fileManage.Value.SetFileDeleteAsync(oldIcon);

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
                await _fileManage.Value.SetFileDeleteAsync(data.Icon);

            await _category.DeleteAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnumsDto<int>>> GetCategoryEnumsAsync()
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

