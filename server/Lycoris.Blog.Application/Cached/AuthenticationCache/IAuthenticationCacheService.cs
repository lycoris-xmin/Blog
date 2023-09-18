using Lycoris.Blog.Application.AppService.Authentication.Dtos;
using Lycoris.Blog.Application.Cached.AuthenticationCache.Dtos;

namespace Lycoris.Blog.Application.Cached.AuthenticationCache
{
    public interface IAuthenticationCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="oathCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetLoginOathCodeAsync(string email, string oathCode, LoginValidateDto value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<LoginOathCodeCacheDto?> GetLoginOathCodeAsync(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task RemoveLoginOathCodeAsync(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<LoginUserCacheDto?> GetLoginStateAsync(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SetLoginStateAsync(LoginDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetLoginStateAsync(string token, LoginUserCacheDto value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetLogoutStateAsync(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task UpdateLoginStateAsync(string token, Action<LoginUserCacheDto> configure);
    }
}
