namespace Lycoris.Blog.Application.AppService.Talks.Dtos
{
    public class TalkDataDto
    {
        public long Id { get; set; }

        public string? Content { get; set; }

        public int AgentFlag { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
