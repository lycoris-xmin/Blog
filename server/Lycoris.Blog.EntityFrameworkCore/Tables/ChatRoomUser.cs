using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 聊天室用户表
    /// </summary>
    [Table("ChatRoom_User")]
    [TableIndex("UserId")]
    [TableIndex("RoomId")]
    public class ChatRoomUser : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 聊天室编号
        /// </summary>
        public long RoomId { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }
    }
}
