namespace Lycoris.Blog.Application.AppService.SiteNavigations.Dtos
{
    public class SiteNavigationQueryDataDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Domain { get; set; }

        public string? Group { get; set; }

        public int HrefCount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
