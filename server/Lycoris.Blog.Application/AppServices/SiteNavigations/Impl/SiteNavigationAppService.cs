using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class SiteNavigationAppService : ApplicationBaseService, ISiteNavigationAppService
    {
        private readonly IRepository<SiteNavigationGroup, int> _siteNavigationGroup;
        private readonly IRepository<SiteNavigation, int> _siteNavigation;

        public SiteNavigationAppService(IRepository<SiteNavigationGroup, int> siteNavigationGroup, IRepository<SiteNavigation, int> siteNavigation)
        {
            _siteNavigationGroup = siteNavigationGroup;
            _siteNavigation = siteNavigation;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<SiteNavigationGroupDataDto>> GetSiteNavigationAsync()
        {
            var groups = await _siteNavigationGroup.GetAll().OrderBy(x => x.Order).Select(x => new SiteNavigationGroupDataDto { GroupId = x.Id, GroupName = x.GroupName }).ToListAsync();

            if (!groups.HasValue())
                return new List<SiteNavigationGroupDataDto>();

            var query = _siteNavigation.GetAll()
                                       .OrderBy(x => x.Order)
                                       .ThenBy(x => x.CreateTime)
                                       .Select(x => new SiteNavigationDataDto()
                                       {
                                           Name = x.Name,
                                           Domain = x.Domain,
                                           Url = x.Url,
                                           GroupId = x.GroupId,
                                       });

            var list = await query.ToListAsync();

            foreach (var item in groups)
                item.List = list.Where(x => x.GroupId == item.GroupId).ToList();

            return groups.Where(x => x.List.HasValue()).ToList();
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<SiteNavigationQueryDataDto>> GetListAsync(SiteNavigationQueryFilter input)
        {
            var filter = _siteNavigation.GetAll()
                                        .WhereIf(!input.Name.IsNullOrEmpty(), x => EF.Functions.Like(x.Name, $"%{input.Name!}%"))
                                        .WhereIf(input.GroupId.HasValue && input.GroupId.Value > 0, x => x.GroupId == input.GroupId!.Value)
                                        .WhereIf(!input.Url.IsNullOrEmpty(), x => EF.Functions.Like(x.Url, $"%{input.Url!}%"));

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<SiteNavigationQueryDataDto>(count);

            var query = from site in filter.OrderByDescending(x => x.CreateTime).PageBy(input.PageIndex, input.PageSize)

                        join g in _siteNavigationGroup.GetAll() on site.GroupId equals g.Id into gg
                        from @group in gg.DefaultIfEmpty()

                        select new SiteNavigationQueryDataDto()
                        {
                            Id = site.Id,
                            Name = site.Name,
                            Url = site.Url,
                            GroupId = @group != null ? @group.Id : 0,
                            GroupName = @group != null ? @group.GroupName : "未分组",
                            HrefCount = site.HrefCount,
                            CreateTime = site.CreateTime
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<SiteNavigationQueryDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SiteNavigationQueryDataDto> CreateAsync(CreateSiteNavigationDto input)
        {
            SiteNavigationGroup group;
            if (input.GroupId.HasValue && input.GroupId.Value > 0)
                group = await _siteNavigationGroup.GetAsync(input.GroupId!.Value) ?? throw new FriendlyException($"{input.GroupName} 分组不存在");
            else
            {
                group = await _siteNavigationGroup.CreateAsync(new SiteNavigationGroup()
                {
                    GroupName = input.GroupName!,
                    Order = 999
                });
            }

            var data = new SiteNavigation()
            {
                Name = input.Name!,
                Domain = UrlHelper.GetUrlPrefix(input.Url!),
                Url = input.Url!,
                GroupId = group.Id,
                CreateTime = DateTime.Now,
                Order = 999
            };

            var repeat = await _siteNavigation.GetAll().Where(x => x.Domain == data.Domain).AnyAsync();
            if (repeat)
                throw new FriendlyException("网站已收录");

            data = await _siteNavigation.CreateAsync(data);

            var dto = data.ToMap<SiteNavigationQueryDataDto>();

            dto.GroupId = group.Id;
            dto.GroupName = group.GroupName;

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SiteNavigationQueryDataDto> UpdateAsync(UpdateSiteNavigationDto input)
        {
            var data = await _siteNavigation.GetAsync(input.Id) ?? throw new FriendlyException("");

            var fiedIds = new List<Expression<Func<SiteNavigation, object>>>();

            if (!input.Url.IsNullOrEmpty() && data.Url != input.Url)
            {
                var repeat = await _siteNavigation.GetAll().Where(x => x.Url == input.Url).AnyAsync();
                if (repeat)
                    throw new FriendlyException("网站已收录");

                data.Domain = UrlHelper.GetUrlPrefix(input.Url!);
                fiedIds.Add(x => x.Domain);

                data.Url = input.Url!;
                fiedIds.Add(x => x.Url);
            }

            data.UpdatePorpertyIf(!input.Name.IsNullOrEmpty() && data.Name != input.Name, x =>
            {
                x.Name = input.Name!;
                fiedIds.Add(x => x.Name);
            });

            SiteNavigationGroup? group = null;
            if (!input.GroupId.HasValue || input.GroupId.Value != data.GroupId)
            {
                if (input.GroupId.HasValue && input.GroupId.Value > 0)
                    group = await _siteNavigationGroup.GetAsync(input.GroupId!.Value) ?? throw new FriendlyException($"{input.GroupName} 分组不存在");
                else
                {
                    group = await _siteNavigationGroup.CreateAsync(new SiteNavigationGroup()
                    {
                        GroupName = input.GroupName!,
                        Order = 999
                    });
                }

                data.GroupId = group.Id;
                fiedIds.Add(x => x.GroupId);
            }
            else if (data.GroupId > 0)
                group = await _siteNavigationGroup.GetAsync(data.GroupId);

            if (fiedIds.HasValue())
                await _siteNavigation.UpdateFieIdsAsync(data, fiedIds);

            var dto = data.ToMap<SiteNavigationQueryDataDto>();

            dto.GroupId = group?.Id ?? 0;
            dto.GroupName = group?.GroupName ?? "未分类";

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var data = await _siteNavigation.GetAsync(id);
            if (data == null)
                return;

            await _siteNavigation.DeleteAsync(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task SetGroupOrderAsync(params int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return;

            var groups = await _siteNavigationGroup.GetAll().ToListAsync();

            for (int i = 0; i < ids.Length; i++)
            {
                var item = groups.SingleOrDefault(x => x.Id == ids[i]);
                if (item == null)
                    continue;

                item.Order = i + 1;
            }

            await _siteNavigationGroup.UpdateAsync(groups);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteGroupAsync(int id)
        {
            var group = await _siteNavigationGroup.GetAsync(id);
            if (group == null)
                return;

            var groupHasSite = await _siteNavigation.GetAll().Where(x => x.GroupId == group.Id).AnyAsync();
            if (groupHasSite)
                throw new FriendlyException($"{group.GroupName} 分组下存在收录数据无法删除");

            await _siteNavigationGroup.DeleteAsync(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnumsDto<int>>> GetSiteNavigationGroupListAsync()
        {
            var filter = _siteNavigationGroup.GetAll().OrderBy(x => x.Order);
            var query = filter.Select(x => new EnumsDto<int>()
            {
                Name = x.GroupName,
                Value = x.Id
            });

            return await query.ToListAsync();
        }
        #endregion
    }
}
