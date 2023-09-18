using Lycoris.Blog.Application.AppService.Authentication.Dtos;
using Lycoris.Blog.Application.Cached.AuthenticationCache.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Core.Interceptors.Transactional;

namespace Lycoris.Blog.Application.AppService.Authentication
{
    public interface IAuthenticationAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<LoginValidateDto> LoginValidateAsync(string account, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remember"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        Task<LoginDto> LoginAsync(LoginValidateDto input, bool remember, bool isManagement);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync(bool isManagement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        Task<LoginDto> RefreshTokenAsync(string refreshToken, bool isManagement);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<LoginDto> DashboardSSOLoginAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isManagement"></param>
        /// <returns></returns>
        Task<LoginUserCacheDto?> GetLoginUserAsync(string token, bool isManagement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> CheckEmailUseAsync(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Transactional]
        Task RegisterAsync(RegisterDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> GetAdminPathAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task ChangePasswordAsync(ChangePasswordDto input);
    }
}
