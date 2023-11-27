using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.PostComments.Dtos
{
    public class PostCommentListFilter : PageFilter
    {
        public long PostId { get; set; }
    }
}
