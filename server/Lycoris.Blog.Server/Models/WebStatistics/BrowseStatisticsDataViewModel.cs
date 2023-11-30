namespace Lycoris.Blog.Server.Models.WebStatistics
{
    /// <summary>
    /// 
    /// </summary>
    public class BrowseStatisticsDataViewModel
    {
        /// <summary>
        /// 路由地址
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string? PageName { get; set; }

        /// <summary>
        /// 统计总数
        /// </summary>
        public int Count { get; set; }
    }
}
