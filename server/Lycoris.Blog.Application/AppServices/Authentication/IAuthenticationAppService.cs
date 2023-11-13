using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Application.Cached.Authentication.Models;
using Lycoris.Blog.Application.Shared;

namespace Lycoris.Blog.Application.AppServices.Authentication
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
        Task<LoginUserCacheModel?> GetLoginUserAsync(string token, bool isManagement);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        Task ScreenUnLockAsync(string password);
    }
}
