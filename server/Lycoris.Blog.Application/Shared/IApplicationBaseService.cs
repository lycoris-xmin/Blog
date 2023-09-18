using Lycoris.Blog.Model.Contexts;

namespace Lycoris.Blog.Application.Shared
{
    public interface IApplicationBaseService
    {
        Lazy<RequestContext> RequestContext { get; set; }
    }
}
