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
        public List<CommonStatisticsConfiguration> BrowserStatistics { get; set; } = new List<CommonStatisticsConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        public List<CommonStatisticsConfiguration> OSStatistics { get; set; } = new List<CommonStatisticsConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        public List<CommonStatisticsConfiguration> DeviceStatistics { get; set; } = new List<CommonStatisticsConfiguration>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommonStatisticsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }
}
