namespace Lycoris.Blog.Application.SignalR.Dashboard.Dtos
{
    public class ServerMonitorDto
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MonitorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Time { get => this.MonitorTime?.ToString("HH:mm:ss") ?? "00:00:00"; }

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
