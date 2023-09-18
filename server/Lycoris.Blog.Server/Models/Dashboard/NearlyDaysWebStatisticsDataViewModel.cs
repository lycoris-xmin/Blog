namespace Lycoris.Blog.Server.Models.Dashboard
{
    /// <summary>
    /// 
    /// </summary>
    public class NearlyDaysWebStatisticsDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Day { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PVBrowse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UVBrowse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Api { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ErrorApi { get; set; }
    }
}
