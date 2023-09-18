using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 登录失败信息表
    /// </summary>
    [Table("Login_Record_Failed")]
    [TableIndex(new[] { "Email" }, true)]
    public class LoginFailedRecord : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 邮箱帐号
        /// </summary>
        [TableColumn(StringLength = 255, Sensitive = true)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 失败次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 冻结时间
        /// </summary>
        public DateTime? FreezeTime { get; set; }
    }
}
