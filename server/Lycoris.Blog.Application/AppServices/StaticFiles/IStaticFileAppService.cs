﻿using Lycoris.Blog.Application.AppServices.StaticFiles.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

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
    }
}
