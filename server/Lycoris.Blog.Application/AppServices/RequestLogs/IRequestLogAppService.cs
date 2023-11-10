using Lycoris.Blog.Application.AppServices.RequestLogs.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.RequestLogs
{
    public interface IRequestLogAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<RequestLogDataDto>> GetListAsync(GetRequestLogListFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RequestLogInfoDto> GetInfoAsync(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task DeleteAsync(params long[] ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        Task SetAccessControlAsync(string ip);
    }
}
