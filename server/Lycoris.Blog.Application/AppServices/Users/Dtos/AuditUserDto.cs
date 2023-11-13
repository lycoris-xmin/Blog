using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Users.Dtos
{
    public class AuditUserDto
    {
        public long Id { get; set; }

        public UserStatusEnum Status { get; set; }

        public string? Remark { get; set; }
    }
}
