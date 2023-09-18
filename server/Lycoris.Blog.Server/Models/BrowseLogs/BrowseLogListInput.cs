using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.BrowseLogs
{
    /// <summary>
    /// 
    /// </summary>
    public class BrowseLogListInput : PageInput
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
        public string? Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint? Ip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Referer { get; set; }
    }
}
