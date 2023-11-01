namespace Lycoris.Blog.Application.AppServices.Comment.Dtos
{
    public class PostCommentQueryDataDto
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? OriginalContent { get; set; }

        public bool IsOwner { get; set; }

        public string? UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public string? IpAddress { get; set; }
    }
}
