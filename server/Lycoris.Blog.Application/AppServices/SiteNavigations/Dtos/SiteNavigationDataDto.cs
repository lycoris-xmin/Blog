namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class SiteNavigationDataDto
    {
        public string? Group { get; set; }

        public List<SiteNavigationDomainDataDto> GroupList { get; set; } = new List<SiteNavigationDomainDataDto>();
    }

    public class SiteNavigationDomainDataDto
    {
        public string? Name { get; set; }

        public string? Domain { get; set; }
    }
}
