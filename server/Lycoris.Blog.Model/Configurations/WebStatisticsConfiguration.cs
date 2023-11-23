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
        public List<WebBrowseWordMapConfiguration> BrowseWordMap { get; set; } = new List<WebBrowseWordMapConfiguration>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebBrowseWordMapConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public WebBrowseWordMapConfiguration() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Country"></param>
        public WebBrowseWordMapConfiguration(string Country)
        {
            this.Country = Country;
            this.Count = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Country"></param>
        /// <param name="Count"></param>
        public WebBrowseWordMapConfiguration(string Country, int Count)
        {
            this.Country = Country;
            this.Count = Count;
        }

        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// 统计总数
        /// </summary>
        public int Count { get; set; } = 0;
    }
}
