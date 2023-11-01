using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class SiteNavigationQueryFilter : PageFilter
    {
        public string? Name { get; set; }

        public string? Group { get; set; }

        public string? Domain { get; set; }
    }
}
