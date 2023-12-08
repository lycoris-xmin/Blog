namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class CreateSiteNavigationDto
    {
        public string? Name { get; set; }

        public string? Url { get; set; }

        public int? GroupId { get; set; }

        public string? GroupName { get; set; }
    }
}
