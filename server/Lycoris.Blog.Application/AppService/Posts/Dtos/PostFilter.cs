using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppService.Posts.Dtos
{
    public class PostFilter : PageFilter
    {
        public string? Title { get; set; }

        public int? Category { get; set; }
    }
}
