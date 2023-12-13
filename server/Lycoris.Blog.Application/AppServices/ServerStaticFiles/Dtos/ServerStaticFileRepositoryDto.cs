using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.ServerStaticFiles.Dtos
{
    public class ServerStaticFileRepositoryDto
    {
        public string? FileName { get; set; }

        public string? Url { get; set; }

        public long FileSize { get; set; }

        public FileTypeEnum FileType { get; set; }
    }
}
