namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class SiteNavigationQueryDataDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Url { get; set; }

        public int? GroupId { get; set; }

        public string? GroupName { get; set; }

        public int HrefCount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
