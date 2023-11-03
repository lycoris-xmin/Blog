using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Application.AppServices.StaticFiles.Dtos
{
    public class StaticFileListFilter : PageFilter
    {
        public FileSaveChannelEnum? SaveChannel { get; set; }

        public bool? Use { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
