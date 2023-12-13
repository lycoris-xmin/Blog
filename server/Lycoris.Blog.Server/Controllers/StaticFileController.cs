using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.FileManage;
using Lycoris.Blog.Application.AppServices.ServerStaticFiles;
using Lycoris.Blog.Application.AppServices.ServerStaticFiles.Dtos;
using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Exceptions;
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
    /// 文件管理
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Repository")]
        [Produces("application/json")]
        public async Task<PageOutput<StaticFileRepositoryDataViewModel>> StaticFileRepository([FromQuery] StaticFileRepositoryInput input)
        {
            FileTypeEnum? fileType = null;
            if (input.FileType.HasValue)
            {
                var enumValues = typeof(FileTypeEnum).GetEnumValues() as int[];
                if (!enumValues!.Contains(input.FileType.Value))
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "");

                fileType = (FileTypeEnum)input.FileType;
            }

            var dto = await _staticFile.GetServerStaticFileRepositoryAsync(input.PageIndex!.Value, input.PageSize!.Value, fileType);

            return Success(dto.Count, dto.List.ToMapList<StaticFileRepositoryDataViewModel>());
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fileManage"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        [Consumes("multipart/form-data"), Produces("application/json")]
        public async Task<DataOutput<StaticFileUploadViewModel>> Upload([FromForm] StaticFileUploadInput input, [FromServices] IFileManageAppService fileManage)
        {
            var url = "";
            FileTypeEnum? fileType = null;
            switch (input.UploadType!.Value)
            {
                case UploadType.PostIcon:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.PostIcon);
                    break;
                case UploadType.PostCarousel:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.PostCarousel);
                    break;
                case UploadType.Post:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.Post);
                    break;
                case UploadType.CategoryIcon:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.Category);
                    break;
                case UploadType.AboutWeb:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.AboutWeb);
                    break;
                case UploadType.Logo:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.Logo);
                    break;
                case UploadType.Avatar:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.Avatar);
                    break;
                case UploadType.Other:
                    (url, fileType) = await fileManage.UploadFileAsync(input.File!, StaticsFilePath.File);
                    break;
                default:
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, "");
            }

            return Success(new StaticFileUploadViewModel(url, (int)fileType!));
        }
    }
}
