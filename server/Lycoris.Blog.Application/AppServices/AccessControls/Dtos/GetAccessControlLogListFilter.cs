using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.AccessControls.Dtos
{
    public class GetAccessControlLogListFilter : PageFilter
    {
        public int AccessControlId { get; set; }
    }
}
