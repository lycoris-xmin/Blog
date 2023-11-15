using Lycoris.Blog.Model.Global.Input;
using Lycoris.Blog.Server.PropertyAttribute;

namespace Lycoris.Blog.Server.Models.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UserListInput : PageInput
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailRegex]
        public string? Email { get; set; }
    }
}
