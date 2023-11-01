using Lycoris.Blog.Application.AppServices.Talks.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Talks
{
    public interface ITalkAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResultDto<TalkDataDto>> GetTalkListAsync(int pageIndex, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResultDto<MasterTalkDataDto>> GetListAsync(int pageIndex, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MasterTalkDataDto> CreateOrUpdateAsync(string content, long id = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeletaAsync(long id);
    }
}
