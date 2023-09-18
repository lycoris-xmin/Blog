using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.CloudStorage.Minio;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppService.Users.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true, EnableInterceptor = true)]
    public class UserAppService : ApplicationBaseService, IUserAppService
    {
        private readonly IRepository<User, long> _user;
        private readonly IRepository<UserLink, long> _userLink;
        private readonly Lazy<IMinioService> _minio;

        /// <summary>
        /// 
        /// </summary>
        public UserAppService(IRepository<User, long> user, IRepository<UserLink, long> userLink, Lazy<IMinioService> minio)
        {
            _user = user;
            _userLink = userLink;
            _minio = minio;
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

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task UpdateUserBrieAsync(UserBriefDto input, IFormFile? file)
        {
            // 用户
            var user = await _user.GetAsync(CurrentUser.Id) ?? throw new FriendlyException("");
            if (file != null)
            {
                input.Avatar = await _minio.Value.UploadFileAsync(x =>
                {
                    x.WithBucketPath("/avatar");
                    x.WithFormFile(file!);
                });
            }

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

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion
    }
}
