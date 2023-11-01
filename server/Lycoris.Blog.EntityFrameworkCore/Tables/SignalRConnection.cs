using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// SignalR 客户端连接记录表
    /// </summary>
    [Table("SignalR_Connection")]
    [TableIndex("ConnectionId", true)]
    [TableIndex("DisconnectedTime")]
    [TableIndex(new[] { "UserId", "Online" })]
    public class SignalRConnection : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 客户端连接Id
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string ConnectionId { get; set; } = "";

        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; } = "";

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; } = "";

        /// <summary>
        /// 客户端是否在线
        /// </summary>
        public bool Online { get; set; }

        /// <summary>
        /// 连线时间
        /// </summary>
        public DateTime ConnectedTime { get; set; }

        /// <summary>
        /// 离线时间
        /// </summary>
        [TableColumn(Required = false)]
        public DateTime? DisconnectedTime { get; set; }
    }
}
