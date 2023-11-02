﻿using Lycoris.Blog.Application.Shared;
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
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file, string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathUrl"></param>
        /// <returns></returns>
        Task SetFileDeleteAsync(string pathUrl);
    }
}
