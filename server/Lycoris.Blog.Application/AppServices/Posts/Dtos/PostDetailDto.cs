using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostDetailDto
    {
        public string? Title { get; set; }

        public string? Markdown { get; set; }

        public PostTypeEnum Type { get; set; }

        public int Category { get; set; }

        public string? CategoryName { get; set; }

        public string? Tags { get; set; }

        public bool Comment { get; set; }

        public DateTime PublishTime { get; set; }

        public int Days { get => (int)Math.Floor((DateTime.Now - PublishTime).TotalDays); }

        public int Browse { get; set; }
    }
}
