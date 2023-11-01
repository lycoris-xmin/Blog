namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostRecommendDataDto
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Info { get; set; }

        public bool Recommend { get; set; } = false;
    }
}
