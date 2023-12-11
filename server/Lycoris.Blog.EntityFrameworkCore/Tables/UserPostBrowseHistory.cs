using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 用户浏览历史表
    /// </summary>
    [Table("User_Post_BrowseHistory")]
    [TableIndex(new string[] { "PostId", "UserId" }, true)]
    [TableIndex("LastTime")]
    public class UserPostBrowseHistory : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 文章编号
        /// </summary>
        public long PostId { get; set; }

        /// <summary>
        /// 浏览用户
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime LastTime { get; set; }
    }
}
