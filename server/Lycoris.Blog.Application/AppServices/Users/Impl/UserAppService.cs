using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.Users.Dtos;
using Lycoris.Blog.Application.Cached.Authentication;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppServices.Users.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true, EnableInterceptor = true)]
    public class UserAppService : ApplicationBaseService, IUserAppService
    {
        private readonly IRepository<User, long> _user;
        private readonly IRepository<UserLink, long> _userLink;
        private readonly Lazy<IRepository<LoginToken, int>> _loginToken;
        private readonly Lazy<IFileManageAppService> _fileManage;
        private readonly Lazy<IAuthenticationCacheService> _cache;
        private readonly Lazy<IScheduleQueueCacheService> _scheduleQueueCache;

        public UserAppService(IRepository<User, long> user,
                              IRepository<UserLink, long> userLink,
                              Lazy<IRepository<LoginToken, int>> loginToken,
                              Lazy<IFileManageAppService> fileManage,
                              Lazy<IAuthenticationCacheService> cache,
                              Lazy<IScheduleQueueCacheService> scheduleQueueCache)
        {
            _user = user;
            _userLink = userLink;
            _loginToken = loginToken;
            _fileManage = fileManage;
            _cache = cache;
            _scheduleQueueCache = scheduleQueueCache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<UserBriefDto?> GetUserBriefAsync()
        {
            if (!LoginState)
                return null;

            return await GetUserBriefAsync(CurrentUser.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<UserBriefDto?> GetUserBriefAsync(long userId)
        {
            var dto = await _user.GetSelectAsync(userId, x => new UserBriefDto()
            {
                Id = x.Id,
                NickName = x.NickName,
                Avatar = x.Avatar,
                Email = x.Email,
                IsAdmin = x.IsAdmin
            });

            if (dto == null)
                return null;

            var link = await _userLink.GetAsync(dto.Id!.Value);

            dto.Github = link?.Github;
            dto.WeChat = link?.WeChat;
            dto.QQ = link?.QQ;
            dto.Gitee = link?.Gitee;
            dto.CloudMusic = link?.CloudMusic;
            dto.Bilibili = link?.Bilibili;

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Transactional]
        public async Task UpdateUserBrieAsync(UserBriefDto input)
        {
            // 用户
            var user = await _user.GetAsync(CurrentUser.Id) ?? throw new FriendlyException("");

            var userFieIds = new List<Expression<Func<User, object>>>();
            user.UpdatePorpertyIf(!input.NickName.IsNullOrEmpty() && input.NickName != user!.NickName, x =>
            {
                user!.NickName = input.NickName!.Trim();
                userFieIds.Add(x => x.NickName!);
            }).UpdatePorpertyIf(!input.Avatar.IsNullOrEmpty() && input.Avatar != user!.Avatar, x =>
            {
                user!.Avatar = input.Avatar!.Trim();
                userFieIds.Add(x => x.Avatar!);
            });

            await _user.UpdateFieIdsAsync(user, userFieIds);

            // 第三方绑定
            var userLink = await _userLink.GetAsync(CurrentUser.Id) ?? new UserLink();
            var userLinkFieIds = new List<Expression<Func<UserLink, object>>>();
            userLink.UpdatePorpertyIf(input.QQ != null && input.QQ != userLink!.QQ, x =>
            {
                userLink!.QQ = input.QQ!.Trim(); ;
                userLinkFieIds.Add(x => x.QQ!);
            }).UpdatePorpertyIf(input.WeChat != null && input.WeChat != userLink!.WeChat, x =>
            {
                userLink!.WeChat = input.WeChat!.Trim();
                userLinkFieIds.Add(x => x.WeChat!);
            }).UpdatePorpertyIf(input.Github != null && input.Github != userLink!.Github, x =>
            {
                userLink!.Github = input.Github!.TrimEnd('/').Trim();
                userLinkFieIds.Add(x => x.Github!);
            }).UpdatePorpertyIf(input.Gitee != null && input.Gitee != userLink!.Gitee, x =>
            {
                userLink!.Gitee = input.Gitee!.TrimEnd('/').Trim();
                userLinkFieIds.Add(x => x.Gitee!);
            }).UpdatePorpertyIf(input.CloudMusic != null && input.CloudMusic != userLink!.CloudMusic, x =>
            {
                userLink!.CloudMusic = input.CloudMusic!.TrimEnd('/').Trim();
                userLinkFieIds.Add(x => x.CloudMusic!);
            }).UpdatePorpertyIf(input.Bilibili != null && input.Bilibili != userLink!.Bilibili, x =>
            {
                userLink!.Bilibili = input.Bilibili!.TrimEnd('/').Trim();
                userLinkFieIds.Add(x => x.Bilibili!);
            });

            if (userLink.Id == 0)
            {
                userLink.Id = user.Id;
                await _userLink.CreateAsync(userLink);
            }
            else
                await _userLink.UpdateFieIdsAsync(userLink, userLinkFieIds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<UserDataDto>> GetListAsync(GetUserListFilter input)
        {
            var filter = _user.GetAll().Where(x => x.IsAdmin == false)
                              .WhereIf(!input.NickName.IsNullOrEmpty(), x => EF.Functions.Like(x.NickName, $"%{input.NickName}%"))
                              .WhereIf(!input.Email.IsNullOrEmpty(), x => x.Email == input.Email);

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<UserDataDto>(count);

            var query = filter.OrderByDescending(x => x.Id)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new UserDataDto()
                              {
                                  Id = x.Id,
                                  Email = x.Email,
                                  NickName = x.NickName,
                                  Avatar = x.Avatar,
                                  Status = x.Status,
                                  CreateTime = x.CreateTime
                              });

            var list = await query.ToListAsync();

            return new PageResultDto<UserDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserLinkDto> GetUserLinkAsync(long userId)
        {
            var data = await _userLink.GetAsync(userId);
            return data?.ToMap<UserLinkDto>() ?? new UserLinkDto();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<EnumsDto<int>> GetUserStatusEnums()
        {
            return Enum.GetValues<UserStatusEnum>().Select(x => new EnumsDto<int>
            {
                Value = (int)x,
                Name = x.GetEnumDescription<UserStatusEnum>()
            }).ToList();
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<UserDataDto> CreateUserAsync(CreateUserDto input)
        {
            var repeat = await _user.GetAll().Where(x => x.Email == input.Email).AnyAsync();
            if (repeat)
                throw new FriendlyException("邮箱已被注册");

            var setting = await this.ApplicationConfiguration.Value.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSetting);

            var data = new User()
            {
                Email = input.Email!,
                NickName = input.NickName!,
                Password = input.Password ?? "",
                Avatar = setting!.DefaultAvatar,
                Status = UserStatusEnum.Audited,
                CreateTime = DateTime.Now
            };

            data = await _user.CreateAsync(data);

            _scheduleQueueCache.Value.Enqueue(ScheduleTypeEnum.WebStatistics, new WebStatisticsQueueModel() { User = 1 });

            return data.ToMap<UserDataDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        [Transactional]
        public async Task AuditUserAsync(AuditUserDto input)
        {
            var data = await _user.GetAsync(input.Id) ?? throw new FriendlyException("用户不存在", $"can not find user by id:{input.Id}");

            if (data.Status != input.Status)
                data.Status = input.Status;

            data.Remark = input.Remark ?? "";

            await _user.UpdateFieIdsAsync(data, x => x.Status, x => x.Remark);

            // 缓存处理
            var token = await _loginToken.Value.GetAll().Where(x => x.UserId == data.Id).Where(x => x.IsManagement == false).SingleOrDefaultAsync();

            if (token == null || (token.TokenExpireTime <= DateTime.Now && token.RefreshTokenExpireTime <= DateTime.Now))
                return;

            // 更新令牌
            _cache.Value.UpdateLoginState(token.Token, x => x.Status = data.Status);
        }
    }
}
