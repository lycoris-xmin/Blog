namespace Lycoris.Blog.Server.Models.BrowseLogs
{
    /// <summary>
    /// 
    /// </summary>
    public class BrowseLogDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Referer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
