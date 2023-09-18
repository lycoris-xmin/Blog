using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.FriendLinks
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendLinkQueryListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Status { get; set; }
    }
}
