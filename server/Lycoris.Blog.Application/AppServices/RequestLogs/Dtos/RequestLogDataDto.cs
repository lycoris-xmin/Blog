namespace Lycoris.Blog.Application.AppServices.RequestLogs.Dtos
{
    public class RequestLogDataDto
    {
        public long Id { get; set; }

        public string? HttpMethod { get; set; }

        public string? Route { get; set; }

        public bool Success { get; set; }

        public uint StatusCode { get; set; }

        public int ElapsedMilliseconds { get; set; }

        public uint Ip { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
