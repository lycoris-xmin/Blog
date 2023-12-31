﻿using Lycoris.Blog.Application.AppServices.BrowseLogs.Dtos;
using Lycoris.Blog.Application.Shared;
using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.BrowseLogs
{
    public interface IBrowseLogAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<BrowseLogDataDto>> GetListAsync(BrowseLogListFilter input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task DeleteAsync(params long[] ids);
    }
}
