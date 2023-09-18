using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.SiteNavigations.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppService.SiteNavigations.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class SiteNavigationAppService : ApplicationBaseService, ISiteNavigationAppService
    {
        private readonly IRepository<SiteNavigation, int> _siteNavigation;

        public SiteNavigationAppService(IRepository<SiteNavigation, int> siteNavigation)
        {
            _siteNavigation = siteNavigation;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<SiteNavigationDataDto>> GetSiteNavigationListAsync()
        {
            var query = _siteNavigation.GetAll()
                                       .GroupBy(x => x.Group)
                                       .Select(x => new SiteNavigationDataDto()
                                       {
                                           Group = x.Key,
                                           GroupList = x.Select(x => new SiteNavigationDomainDataDto()
                                           {
                                               Name = x.Name,
                                               Domain = x.Domain
                                           }).ToList()
                                       });

            return await query.ToListAsync();
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
                                        .WhereIf(!input.Group.IsNullOrEmpty(), x => x.Group == input.Group)
                                        .WhereIf(!input.Domain.IsNullOrEmpty(), x => EF.Functions.Like(x.Domain, $"%{input.Domain!}%"));

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<SiteNavigationQueryDataDto>(count);

            var query = filter.OrderByDescending(x => x.CreateTime)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new SiteNavigationQueryDataDto()
                              {
                                  Id = x.Id,
                                  Name = x.Name,
                                  Domain = x.Domain,
                                  Group = x.Group,
                                  HrefCount = x.HrefCount,
                                  CreateTime = x.CreateTime
                              });

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
            var data = new SiteNavigation()
            {
                Name = input.Name!,
                Domain = input.Domain!,
                Group = input.Group!,
                CreateTime = DateTime.Now
            };

            var repeat = await _siteNavigation.GetAll().Where(x => x.Domain == data.Domain).AnyAsync();
            if (repeat)
                throw new FriendlyException("网站已收录");

            data = await _siteNavigation.CreateAsync(data);

            return data.ToMap<SiteNavigationQueryDataDto>();
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

            if (!input.Domain.IsNullOrEmpty() && data.Domain != input.Domain)
            {
                var repeat = await _siteNavigation.GetAll().Where(x => x.Domain == input.Domain).AnyAsync();
                if (repeat)
                    throw new FriendlyException("网站已收录");

                data.Domain = input.Domain!;
                fiedIds.Add(x => x.Domain);
            }

            data.UpdatePorpertyIf(!input.Name.IsNullOrEmpty() && data.Name != input.Name, x =>
            {
                x.Name = input.Name!;
                fiedIds.Add(x => x.Name);
            }).UpdatePorpertyIf(!input.Group.IsNullOrEmpty() && data.Group != input.Group, x =>
            {
                x.Group = input.Group!;
                fiedIds.Add(x => x.Group);
            });

            if (fiedIds.HasValue())
                await _siteNavigation.UpdateFieIdsAsync(data, fiedIds);

            return data.ToMap<SiteNavigationQueryDataDto>();
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
        /// <returns></returns>
        public async Task<List<EnumsDto<string>>> GetSiteNavigationGroupAsync()
        {
            var filter = _siteNavigation.GetAll().Select(x => x.Group).Distinct();
            var query = filter.Select(x => new EnumsDto<string>()
            {
                Name = x,
                Value = x
            });

            return await query.ToListAsync();
        }
        #endregion
    }
}
