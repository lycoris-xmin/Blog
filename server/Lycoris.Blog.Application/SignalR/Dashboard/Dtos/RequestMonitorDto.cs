namespace Lycoris.Blog.Application.SignalR.Dashboard.Dtos
{
    public class RequestMonitorDto
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MonitorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Time { get => this.MonitorTime?.ToString("HH:mm") ?? "00:00"; }

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
