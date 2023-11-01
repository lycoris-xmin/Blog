namespace Lycoris.Blog.Application.SignalR.Models
{
    public class RequestMonitorModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MonitorTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Time { get => MonitorTime?.ToString("HH:mm") ?? "00:00"; }

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
