using Lycoris.Blog.Application.AppServices.Users.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

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
        /// <returns></returns>
        Task UpdateUserBrieAsync(UserBriefDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<UserDataDto>> GetListAsync(GetUserListFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserLinkDto> GetUserLinkAsync(long userId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<EnumsDto<int>> GetUserStatusEnums();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<UserDataDto> CreateUserAsync(CreateUserDto input);

        /// <summary>
        /// 审核用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AuditUserAsync(AuditUserDto input);
    }
}
