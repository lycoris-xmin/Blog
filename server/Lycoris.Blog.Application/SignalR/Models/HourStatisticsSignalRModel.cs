namespace Lycoris.Blog.Application.SignalR.Models
{
    public class HourStatisticsSignalRModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int OnlineUsers { get; set; }

        /// <summary>
        /// 浏览量(PV)
        /// </summary>
        public int PVBrowse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? PVBrowsePercent { get; set; }

        /// <summary>
        /// 访客数(UV)
        /// </summary>
        public int UVBrowse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? UVBrowsePercent { get; set; }

        /// <summary>
        /// 评论、留言
        /// </summary>
        public int CommentMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double CommentMessagePercent { get; set; }

        /// <summary>
        /// 平均响应时间
        /// </summary>
        public int ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? ElapsedMillisecondsDifference { get; set; }
    }
}
