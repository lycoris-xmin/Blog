using Lycoris.Blog.Application.AppServices.LoginRecords.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.LoginRecords
{
    public interface ILoginRecordAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResultDto<LoginRecordDataDto>> GetListAsync(int pageIndex, int pageSize);
    }
}
