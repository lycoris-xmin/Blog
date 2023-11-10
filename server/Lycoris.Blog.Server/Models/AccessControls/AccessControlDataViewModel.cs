namespace Lycoris.Blog.Server.Models.AccessControls
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessControlDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// ip归属地
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 异常访问次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 最后一次访问时间
        /// </summary>
        public DateTime LastAccessTime { get; set; }
    }
}
