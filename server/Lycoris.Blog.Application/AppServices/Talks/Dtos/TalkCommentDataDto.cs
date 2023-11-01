namespace Lycoris.Blog.Application.AppServices.Talks.Dtos
{
    public class TalkCommentDataDto
    {
        public string? Content { get; set; }

        public string? UserAgent { get; set; }

        public string? IpAddress { get; set; }

        public long UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserAvatar { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
