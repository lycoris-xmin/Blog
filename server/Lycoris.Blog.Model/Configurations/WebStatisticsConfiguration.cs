namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class WebStatisticsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalBrowse { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int TotalMessage { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int TotalUsers { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastTime { get; set; }
    }
}
