using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostFilter : PageFilter
    {
        public string? Title { get; set; }

        public int? Category { get; set; }
    }
}
