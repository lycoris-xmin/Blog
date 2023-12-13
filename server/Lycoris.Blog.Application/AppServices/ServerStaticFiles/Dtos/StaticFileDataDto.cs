using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Application.AppServices.ServerStaticFiles.Dtos
{
    public class StaticFileDataDto
    {
        public long Id { get; set; }

        public string Path { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public FileUploadChannelEnum UploadChannel { get; set; }

        public string PathUrl { get; set; } = string.Empty;

        public string RemoteUrl { get; set; } = string.Empty;

        public FileTypeEnum? FileType { get; set; }

        public long FileSize { get; set; }

        public string FileSha { get; set; } = string.Empty;

        public bool LocalBack { get; set; }

        public bool Use { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
