namespace Lycoris.Blog.Application.AppService.BrowseLogs.Dtos
{
    public class BrowseLogDataDto
    {
        public long Id { get; set; }

        public string? Path { get; set; }

        public string? PageName { get; set; }

        public string? UserAgent { get; set; }

        public uint? Ip { get; set; }

        public string? IpAddress { get; set; }

        public string? Referer { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
