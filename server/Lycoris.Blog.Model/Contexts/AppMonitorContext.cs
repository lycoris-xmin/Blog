using Lycoris.Autofac.Extensions;

namespace Lycoris.Blog.Model.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class AppMonitorContext
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> ServerMonitorConnectionIds { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public List<string> HourStatisticsConnectionIds { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public DateTime? BeginRunTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ServerMonitorContext Server { get; set; } = new ServerMonitorContext();

        /// <summary>
        /// 
        /// </summary>
        public List<RequestMonitorContext> Request { get; set; } = new List<RequestMonitorContext>();

        /// <summary>
        /// 
        /// </summary>
        public HourStatisticsMonitorContext HourStatistics { get; set; } = new HourStatisticsMonitorContext();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ServerMonitorContext
    {
        /// <summary>
        /// 
        /// </summary>
        public double TotalRAM { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ServerMonitorDetail> List { get; set; } = new List<ServerMonitorDetail>();

        /// <summary>
        /// 
        /// </summary>
        public int? MnitorCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ServerMonitorDetail
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime MonitorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double CPURate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double RAMRate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestMonitorContext
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime MonitorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Request { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PV { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UV { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HourStatisticsMonitorContext
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
        public double PVBrowsePercent { get; set; }

        /// <summary>
        /// 访客数(UV)
        /// </summary>
        public int UVBrowse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double UVBrowsePercent { get; set; }

        /// <summary>
        /// 评论、留言
        /// </summary>
        public int CommentMessage { get; set; }

        /// <summary>
        /// 评论、留言
        /// </summary>
        public double CommentMessagePercent { get; set; }

        /// <summary>
        /// 平均响应时间
        /// </summary>
        public int ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ElapsedMillisecondsDifference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastTime { get; set; }
    }
}
