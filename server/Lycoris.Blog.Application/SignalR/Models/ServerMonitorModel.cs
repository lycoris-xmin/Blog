namespace Lycoris.Blog.Application.SignalR.Models
{
    public class ServerMonitorModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MonitorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Time { get => MonitorTime?.ToString("HH:mm:ss") ?? "00:00:00"; }

        /// <summary>
        /// CPU使用率
        /// </summary>
        public string? CPURate { get; set; }

        /// <summary>
        /// 内存使用率
        /// </summary>
        public string? RAMRate { get; set; }
    }
}
