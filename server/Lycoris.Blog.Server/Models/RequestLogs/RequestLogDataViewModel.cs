namespace Lycoris.Blog.Server.Models.RequestLogs
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestLogDataViewModel
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
        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IPAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
