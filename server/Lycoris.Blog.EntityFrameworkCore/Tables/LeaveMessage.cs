using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站留言表
    /// </summary>
    [Table("LeaveMessage")]
    [TableIndex("ParentId")]
    [TableIndex("Ip")]
    [TableIndex("Status")]
    [TableIndex("CreateUserId")]
    public class LeaveMessage : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 父级编号
        /// </summary>
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 被回复的用户编号
        /// </summary>
        public long RepliedUserId { get; set; } = 0;

        /// <summary>
        /// 留言内容
        /// </summary>
        [TableColumn(StringLength = 300)]
        public string OriginalContent { get; set; } = "";

        /// <summary>
        /// 留言内容
        /// </summary>
        [TableColumn(StringLength = 300)]
        public string Content { get; set; } = "";

        /// <summary>
        /// 客户端枚举标识
        /// </summary>
        [TableColumn(ColumnType = MySqlType.TINYINT)]
        public int AgentFlag { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public uint Ip { get; set; }

        /// <summary>
        /// IP归属地
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// 二级评论数
        /// </summary>
        public int ReplyCount { get; set; } = 0;

        /// <summary>
        /// 留言数据状态: 0-正常, 1-违规, 2-用户删除
        /// </summary>
        [TableColumn(DefaultValue = LeaveMessageStatusEnum.Default)]
        public LeaveMessageStatusEnum Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Redundancy { get; set; } = string.Empty;

        /// <summary>
        /// 留言用户编号
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
