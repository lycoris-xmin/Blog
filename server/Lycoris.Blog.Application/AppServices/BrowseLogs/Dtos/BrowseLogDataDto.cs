namespace Lycoris.Blog.Application.AppServices.BrowseLogs.Dtos
{
    public class BrowseLogDataDto
    {
        public long Id { get; set; }

        public string? Route { get; set; }

        public string? PageName { get; set; }

        public uint? Ip { get; set; }

        public string? IpAddress { get; set; }

        public string? Referer { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
