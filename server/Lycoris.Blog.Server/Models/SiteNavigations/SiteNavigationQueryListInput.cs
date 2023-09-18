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
        public string? Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Domain { get; set; }
    }
}
