namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class CreateSiteNavigationDto
    {
        public string? Name { get; set; }

        public string? Domain { get; set; }

        public int? Group { get; set; }

        public string? GroupName { get; set; }
    }
}
