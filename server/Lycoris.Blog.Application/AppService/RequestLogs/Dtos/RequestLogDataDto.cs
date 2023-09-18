namespace Lycoris.Blog.Application.AppService.RequestLogs.Dtos
{
    public class RequestLogDataDto
    {
        public long Id { get; set; }

        public string? Route { get; set; }

        public bool Success { get; set; }

        public int ElapsedMilliseconds { get; set; }

        public uint IP { get; set; }

        public string? IPAddress { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
