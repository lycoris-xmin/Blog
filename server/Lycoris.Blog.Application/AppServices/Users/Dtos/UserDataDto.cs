using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Users.Dtos
{
    public class UserDataDto
    {
        public long Id { get; set; }

        public string? Email { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserStatusEnum Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
