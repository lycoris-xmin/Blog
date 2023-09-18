using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppService.Posts.Dtos
{
    public class PostQueryFilter : PageFilter
    {
        public string? Title { get; set; }

        public PostTypeEnum? Type { get; set; }

        public int? Category { get; set; }

        public bool? IsPublish { get; set; }
    }
}
