using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.SiteNavigations
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNavigationQueryListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Url { get; set; }
    }
}
