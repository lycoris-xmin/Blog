using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UserCreateInput
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required, EmailRegex]
        public string? Email { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Required]
        public string? NickName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }
    }
}
