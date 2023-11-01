using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.FriendLinks.Dtos
{
    public class AuditFriendLinkDto
    {
        public int Id { get; set; }

        public FriendLinkStatusEnum Status { get; set; }

        public string? Description { get; set; }

        public string? Remark { get; set; }
    }
}
