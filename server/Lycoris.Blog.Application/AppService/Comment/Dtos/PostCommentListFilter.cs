using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.Comment.Dtos
{
    public class PostCommentListFilter : PageFilter
    {
        public long PostId { get; set; }
    }
}
