using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.BrowseLogs.Dtos
{
    public class BrowseLogListFilter : PageFilter
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Route { get; set; }

        public uint? Ip { get; set; }

        public string? Referer { get; set; }
    }
}
