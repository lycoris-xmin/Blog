using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.Users
{
    public interface IUserAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<UserBriefDto?> GetUserBriefAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserBriefDto?> GetUserBriefAsync(long userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task UpdateUserBrieAsync(UserBriefDto input, IFormFile? file);
    }
}
