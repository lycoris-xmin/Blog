using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.RequestLogs.Dtos
{
    public class GetRequestLogListFilter : PageFilter
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Route { get; set; }

        public uint? Ip { get; set; }

        public int? Elapsed { get; set; }

        public bool? Success { get; set; }
    }
}
