using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Cache.Authentication.Models;

namespace Lycoris.Blog.Cache.Authentication
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
        void SetLoginOathCode(string email, string oathCode, LoginValidateDto value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        LoginOathCodeCacheModel? GetLoginOathCode(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        void RemoveLoginOathCode(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        LoginUserCacheModel? GetLoginState(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        void SetLoginState(string token, LoginUserCacheModel input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        void SetLogoutState(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        void UpdateLoginState(string token, Action<LoginUserCacheModel> configure);
    }
}
