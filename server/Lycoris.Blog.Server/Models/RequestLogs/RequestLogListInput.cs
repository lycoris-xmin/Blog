using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.RequestLogs
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestLogListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Elapsed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Success { get; set; }
    }
}
