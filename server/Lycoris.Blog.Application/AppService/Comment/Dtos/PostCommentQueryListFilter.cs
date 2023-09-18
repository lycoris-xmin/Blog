using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.Comment.Dtos
{
    public class PostCommentQueryListFilter : PageFilter
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public long? UserId { get; set; }
    }
}
