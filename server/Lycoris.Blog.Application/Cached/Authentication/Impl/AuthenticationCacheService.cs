using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Application.Cached.Authentication.Models;
using Lycoris.Blog.Common.Cache;

namespace Lycoris.Blog.Application.Cached.Authentication.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class AuthenticationCacheService : IAuthenticationCacheService
    {
        private readonly Lazy<IMemoryCacheManager> _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        public AuthenticationCacheService(Lazy<IMemoryCacheManager> memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #region 授权码
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public LoginOathCodeCacheModel? GetLoginOathCode(string email) => _memoryCache.Value.GetMemory<LoginOathCodeCacheModel>(GetLoginOathCodeKey(email));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="oathCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetLoginOathCode(string email, string oathCode, LoginValidateDto value)
        {
            var expitedTime = !value.GoogleAuthentication.HasValue || !value.GoogleAuthentication.Value ? 30 : 120;

            _memoryCache.Value.CreateMemory(GetLoginOathCodeKey(email), new LoginOathCodeCacheModel()
            {
                OathCode = oathCode,
                Value = value

            }, DateTime.Now.AddSeconds(expitedTime));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public void RemoveLoginOathCode(string email) => _memoryCache.Value.RemoveMemory(GetLoginOathCodeKey(email));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private static string GetLoginOathCodeKey(string account) => $"OathCode:Login:{account}";
        #endregion

        #region 访问令牌
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public LoginUserCacheModel? GetLoginState(string token) => _memoryCache.Value.GetMemory<LoginUserCacheModel>(GetTokenKey(token));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="input"></param>
        public void SetLoginState(string token, LoginUserCacheModel input) => _memoryCache.Value.CreateMemory(GetTokenKey(token), input, input.TokenExpireTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void SetLogoutState(string token) => _memoryCache.Value.RemoveMemory(GetTokenKey(token));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public void UpdateLoginState(string token, Action<LoginUserCacheModel> configure)
        {
            var key = GetTokenKey(token);

            var value = _memoryCache.Value.GetMemory<LoginUserCacheModel>(key);

            if (value == null || value.TokenExpireTime < DateTime.Now)
                return;

            configure.Invoke(value);

            _memoryCache.Value.CreateMemory(key, value, value.TokenExpireTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string GetTokenKey(string token) => $"Authentication:{token}";
        #endregion
    }
}
