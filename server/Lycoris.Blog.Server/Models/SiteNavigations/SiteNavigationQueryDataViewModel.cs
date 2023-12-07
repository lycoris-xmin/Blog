namespace Lycoris.Blog.Server.Models.SiteNavigations
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNavigationQueryDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int HrefCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
