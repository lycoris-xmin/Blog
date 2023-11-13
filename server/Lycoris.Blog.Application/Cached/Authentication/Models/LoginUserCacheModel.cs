using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.Cached.Authentication.Models
{
    public class LoginUserCacheModel
    {
        public long Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public UserStatusEnum Status { get; set; }

        public bool? IsAdmin { get; set; }

        public DateTime TokenExpireTime { get; set; }
    }
}
