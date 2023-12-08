using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class SiteNavigationQueryFilter : PageFilter
    {
        public string? Name { get; set; }

        public int? GroupId { get; set; }

        public string? Url { get; set; }
    }
}
