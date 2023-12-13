using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.FileManage
{
    public interface IFileManageAppService : IApplicationBaseService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path">上传路径</param>
        /// <param name="notCheck">不检测使用状态</param>
        /// <returns></returns>
        Task<(string url, FileTypeEnum fileType)> UploadFileAsync(IFormFile file, string path, bool notCheck = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ServerStaticFile> UploadLocalToRemoteAsync(ServerStaticFile data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathUrl"></param>
        /// <returns></returns>
        Task SetFileDeleteAsync(string pathUrl);
    }
}
