using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.StaticFiles;
using Lycoris.Blog.Application.AppServices.StaticFiles.Dtos;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.StaticFiles;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/StaticFile")]
    [AppAuthentication]
    public class StaticFileController : BaseController
    {
        private readonly IStaticFileAppService _staticFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staticFile"></param>
        public StaticFileController(IStaticFileAppService staticFile)
        {
            _staticFile = staticFile;
        }

        /// <summary>
        /// 获取静态文件列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [Produces("application/json")]
        public async Task<PageOutput<StaticFileDataViewModel>> List([FromQuery] StaticFileListInput input)
        {
            var filter = input.ToMap<StaticFileListFilter>();
            var dto = await _staticFile.GetListAsync(filter);
            return Success(dto.Count, dto.List.ToMapList<StaticFileDataViewModel>());
        }

        /// <summary>
        /// 验证文件归档状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Check/UseState/{id}")]
        [Produces("application/json")]
        public async Task<BaseOutput> CheckFileUseState(long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "wrong id parameter");

            await _staticFile.CheckFileUseStateAsync(id.Value);

            return Success();
        }

        /// <summary>
        /// 同步文件至远端仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Syncfile/Remote/{id}")]
        [Produces("application/json")]
        public async Task<BaseOutput> SyncFileToRemote(long? id)
        {
            if (!id.HasValue || id.Value <= 0)
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "wrong id parameter");

            await _staticFile.SyncFileToRemoteRepositoryAsync(id.Value);

            return Success();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("Download/File/All")]
        [Produces("application/json")]
        public async Task<DataOutput<string>> DownloadAllFile()
        {
            var fileName = await _staticFile.DownloadAllFileAsync();
            return Success(fileName);
        }
    }
}
