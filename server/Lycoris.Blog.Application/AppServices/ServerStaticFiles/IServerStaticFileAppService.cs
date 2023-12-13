using Lycoris.Blog.Application.AppServices.ServerStaticFiles.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.ServerStaticFiles
{
    public interface IServerStaticFileAppService : IApplicationBaseService
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        Task<PageResultDto<ServerStaticFileRepositoryDto>> GetServerStaticFileRepositoryAsync(int pageIndex, int pageSize, FileTypeEnum? fileType);
    }
}
