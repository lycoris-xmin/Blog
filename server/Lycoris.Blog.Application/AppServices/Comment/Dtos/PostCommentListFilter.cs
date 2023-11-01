using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Comment.Dtos
{
    public class PostCommentListFilter : PageFilter
    {
        public long PostId { get; set; }
    }
}
