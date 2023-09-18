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
        public string? Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int HrefCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
