using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.AccessControls.Dtos
{
    public class GetAccessControlListFilter : PageFilter
    {
        public string? Ip { get; set; }
    }
}
