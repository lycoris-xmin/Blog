using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UserPostBrowseHistoryListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
