using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 聊天记录表
    /// </summary>
    [Table("ChatMessage")]
    [TableIndex("UserId")]
    [TableIndex("RoomId")]
    public class ChatMessage : MySqlBaseEntity<long>
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
        /// 发布信息
        /// </summary>
        [TableColumn(StringLength = 300)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
