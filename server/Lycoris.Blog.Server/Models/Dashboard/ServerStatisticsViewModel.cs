﻿namespace Lycoris.Blog.Server.Models.Dashboard
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerStatisticsViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Browse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Api { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ErrorApi { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalBrowse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalUsers { get; set; }
    }
}
