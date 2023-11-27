using Lycoris.Blog.Common;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Lycoris.Blog.Server.Controllers
{
    /// <summary>
    /// 下载
    /// </summary>
    [Route($"{HostConstant.RoutePrefix}/Download")]
    public class DownloadController : BaseController
    {
        /// <summary>
        /// 临时文件下载
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("StaticFile/All/{fileName}")]
        public async Task<IActionResult> StaticFileAll(string fileName)
        {
            var filePath = Path.Combine(AppSettings.Path.Temp, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            var fileInfo = new FileInfo(filePath);
            var ext = fileInfo.Extension;

            provider.Mappings.TryGetValue(ext, out var contenttype);

            return await Task.FromResult(File(System.IO.File.ReadAllBytes(filePath), contenttype ?? "application/octet-stream", fileInfo.Name));
        }
    }
}
