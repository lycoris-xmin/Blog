using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppService.FriendLinks.Dtos
{
    public class FriendLinkQueryFilter : PageFilter
    {
        public string? Name { get; set; }

        public FriendLinkStatusEnum? Status { get; set; }
    }
}
