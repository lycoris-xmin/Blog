using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FriendLinks.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Lycoris.Common.Utils.SensitiveWord;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.FriendLinks.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class FriendLinkAppService : ApplicationBaseService, IFriendLinkAppService
    {
        private readonly IRepository<FriendLink, int> _friendLink;
        private readonly Lazy<IRepository<User, long>> _user;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendLink"></param>
        /// <param name="user"></param>
        public FriendLinkAppService(IRepository<FriendLink, int> friendLink, Lazy<IRepository<User, long>> user)
        {
            _friendLink = friendLink;
            _user = user;
        }


        #region ======== 博客网站 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FriendLinkDataDto>> GetFriendLinkListAsync(int pageIndex, int pageSize)
        {
            var filter = _friendLink.GetAll().Where(x => x.Status == FriendLinkStatusEnum.Audited);

            var query = filter.OrderByDescending(x => x.RouteLink)
                              .ThenBy(x => x.CreateTime)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new FriendLinkDataDto()
                              {
                                  Name = x.Name,
                                  Icon = x.Icon,
                                  Link = x.Link,
                                  Description = x.Description
                              });

            return await query.ToListAsync();
        }

        /// <summary>
        /// 友情链接申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task FriendLinkApplyAsync(FriendLinkApplyDto input)
        {
            if (!CurrentUser!.IsAdmin)
            {
                var userApplyed = await _friendLink.GetAll().Where(x => x.CreateUserId == CurrentUser!.Id).OrderByDescending(x => x.CreateTime).Select(x => new { x.Link, x.CreateTime }).ToListAsync();

                if (userApplyed.HasValue())
                {
                    if (userApplyed.Any(x => x.CreateTime >= DateTime.Now.AddMinutes(-5)))
                        throw new FriendlyException("请不要在五分钟内提交多次友链申请，谢谢合作");

                    if (userApplyed.Any(x => x.Link == input.Link))
                        return;

                    if (userApplyed.Count >= 5)
                        throw new FriendlyException("未避免用户恶意申请，限制每个用户最多只能提交五次，敬请谅解");
                }
            }

            var data = input.ToMap<FriendLink>();
            data.CreateUserId = CurrentUser!.Id;
            data.Status = CurrentUser.IsAdmin ? FriendLinkStatusEnum.Audited : FriendLinkStatusEnum.Default;

            await _friendLink.CreateAsync(data);
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<FriendLinkQueryDataDto>> GetListAsync(FriendLinkQueryFilter input)
        {
            var filter = _friendLink.GetAll()
                                    .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
                                    .WhereIf(!input.Name.IsNullOrEmpty(), x => EF.Functions.Like(x.Name, $"%{input.Name!}%"));

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<FriendLinkQueryDataDto>(count);

            var query = from link in filter.OrderByDescending(x => x.CreateTime).PageBy(input.PageIndex, input.PageSize)

                        join u in _user.Value.GetAll() on link.CreateUserId equals u.Id into uu
                        from user in uu.DefaultIfEmpty()

                        select new FriendLinkQueryDataDto()
                        {
                            Id = link.Id,
                            Name = link.Name,
                            Icon = link.Icon,
                            Link = link.Link,
                            Description = link.Description,
                            Status = link.Status,
                            Remark = link.Remark,
                            CreateUserName = user != null ? user.NickName : "用户已注销",
                            CreateTime = link.CreateTime
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<FriendLinkQueryDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FriendLinkQueryDataDto> CreateAsync(CreateFriendLinkDto input)
        {
            var data = new FriendLink()
            {
                Name = input.Name,
                Icon = input.Icon,
                Link = input.Link,
                Description = input.Description ?? "",
                Status = FriendLinkStatusEnum.Audited,
                Remark = "",
                CreateUserId = CurrentUser!.Id,
                CreateTime = DateTime.Now
            };

            data = await _friendLink.CreateAsync(data);

            return new FriendLinkQueryDataDto()
            {
                Id = data.Id,
                Name = data.Name,
                Icon = data.Icon,
                Link = data.Link,
                Description = data.Description,
                Status = data.Status,
                Remark = data.Remark,
                CreateUserName = CurrentUser.NickName,
                CreateTime = data.CreateTime
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> AuditAsync(AuditFriendLinkDto input)
        {
            var data = await _friendLink.GetAsync(input.Id) ?? throw new FriendlyException("数据不存在或已被删除");

            data.Status = input.Status;
            if (!input.Description.IsNullOrEmpty())
                data.Description = input.Description!;
            else
                data.Description = SensitiveWordMemoryStore.SensitiveWordsReplace(data.Description);

            if (input.Status == FriendLinkStatusEnum.Reject && !input.Remark.IsNullOrEmpty())
                data.Remark = input.Remark!;
            else
                data.Remark = "";

            await _friendLink.UpdateFieIdsAsync(data, x => x.Status, x => x.Description, x => x.Remark);

            return data.Description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var data = await _friendLink.GetAsync(id);
            if (data == null)
                return;

            await _friendLink.DeleteAsync(data);
        }
        #endregion
    }
}
