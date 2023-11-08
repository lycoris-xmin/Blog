using Lycoris.Blog.Application.AppServices.StaticFiles.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.StaticFiles
{
    public interface IStaticFileAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<StaticFileDataDto>> GetListAsync(StaticFileListFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task CheckFileUseStateAsync(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task SyncFileToRemoteRepositoryAsync(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task UploadLocalFileAsync(long id, IFormFile file);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteLocalFileAsync(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> DownloadAllFileAsync();
    }
}
