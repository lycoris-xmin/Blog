using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.ServerStaticFiles;
using Lycoris.Blog.Application.AppServices.ServerStaticFiles.Dtos;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Models.Shared;
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
        private readonly IServerStaticFileAppService _staticFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staticFile"></param>
        public StaticFileController(IServerStaticFileAppService staticFile)
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
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Check/UseState")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> CheckFileUseState([FromBody] SingleIdInput<long?> input)
        {
            await _staticFile.CheckFileUseStateAsync(input.Id!.Value);
            return Success();
        }

        /// <summary>
        /// 同步文件至远端仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Syncfile/Remote")]
        [Consumes("application/json"), Produces("application/json")]
        public async Task<BaseOutput> SyncFileToRemote([FromBody] SingleIdInput<long?> input)
        {
            await _staticFile.SyncFileToRemoteRepositoryAsync(input.Id!.Value);
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
