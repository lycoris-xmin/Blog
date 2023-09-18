using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 登录令牌表
    /// </summary>
    [Table("LoginToken")]
    [TableIndex(new[] { "UserId", "IsManagement" }, true)]
    public class LoginToken : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        [TableColumn(StringLength = 500)]
        public string Token { get; set; } = "";

        /// <summary>
        /// 访问令牌过期时间
        /// </summary>
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [TableColumn(StringLength = 500)]
        public string RefreshToken { get; set; } = "";

        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        public DateTime RefreshTokenExpireTime { get; set; }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool Remember { get; set; } = false;

        /// <summary>
        /// 是否为管理后台的登录令牌
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool IsManagement { get; set; }
    }
}
