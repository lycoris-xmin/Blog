using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Server.Models.WebStatistics
{
    /// <summary>
    /// 
    /// </summary>
    public class UserAgentStatisticsViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CommonStatisticsConfiguration>? BrowseClient { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CommonStatisticsConfiguration>? OS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CommonStatisticsConfiguration>? Device { get; set; }
    }
}
