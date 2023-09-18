namespace Lycoris.Blog.Server.Models.SiteNavigations
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNavigationDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SiteNavigationDomainDataViewModel>? GroupList { get; set; }
    }
}
