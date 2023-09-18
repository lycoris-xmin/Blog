using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 聊天室表
    /// </summary>
    [Table("ChatRoom")]
    public class ChatRoom : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 最后沟通时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }
    }
}
