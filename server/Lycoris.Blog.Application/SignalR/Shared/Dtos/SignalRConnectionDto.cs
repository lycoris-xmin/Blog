namespace Lycoris.Blog.Application.SignalR.Shared.Dtos
{
    public class SignalRConnectionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
        public bool? Online { get; set; }

        /// <summary>
        /// 连线时间
        /// </summary>
        public DateTime ConnectedTime { get; set; }

        /// <summary>
        /// 离线时间
        /// </summary>
        public DateTime? DisconnectedTime { get; set; }
    }
}
