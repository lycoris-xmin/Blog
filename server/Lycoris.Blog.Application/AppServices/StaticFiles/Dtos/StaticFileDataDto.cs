using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Application.AppServices.StaticFiles.Dtos
{
    public class StaticFileDataDto
    {
        public long Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public FileSaveChannelEnum SaveChannel { get; set; }

        public string PathUrl { get; set; } = string.Empty;

        public string RemoteUrl { get; set; } = string.Empty;

        public string FileSha { get; set; } = string.Empty;

        public bool Use { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
