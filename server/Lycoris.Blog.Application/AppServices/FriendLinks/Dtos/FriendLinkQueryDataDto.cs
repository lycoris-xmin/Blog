using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.FriendLinks.Dtos
{
    public class FriendLinkQueryDataDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Icon { get; set; }

        public string? Link { get; set; }

        public string? Description { get; set; }

        public FriendLinkStatusEnum Status { get; set; }

        public string? Remark { get; set; }

        public string? CreateUserName { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
