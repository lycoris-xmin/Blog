using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 文章评论表
    /// </summary>
    [Table("Post_Comment")]
    [TableIndex("PostId")]
    public class PostComment : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 文章编号
        /// </summary>
        public long PostId { get; set; }

        /// <summary>
        /// 被回复评论用户编号
        /// </summary>
        public long RepliedUserId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [TableColumn(StringLength = 1500)]
        public string Content { get; set; } = "";

        /// <summary>
        /// 客户端枚举标识
        /// </summary>
        [TableColumn(ColumnType = MySqlType.TINYINT)]
        public int AgentFlag { get; set; }

        /// <summary>
        /// 评论用户客户端
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string UserAgent { get; set; } = "";

        /// <summary>
        /// ip地址
        /// </summary>
        public uint Ip { get; set; }

        /// <summary>
        /// Ip归属地
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// 评论用户编号
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 不映射字段
        /// </summary>
        [NotMapped]
        public override byte[]? RowVersion { get; set; }
    }
}
