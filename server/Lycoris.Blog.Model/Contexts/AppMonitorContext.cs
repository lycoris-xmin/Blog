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
        public List<string> ConnectionIds { get; set; } = new List<string>();

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
}
