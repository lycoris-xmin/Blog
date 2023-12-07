using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables;

namespace Lycoris.Blog.Application.AppServices.FreezeUsers
{
    public interface IFreezeUserAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<FreezeUser>> GetFreezeUserListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="failedCount"></param>
        /// <returns></returns>
        Task SetFreezeUserAsync(long id, int failedCount);
    }
}
