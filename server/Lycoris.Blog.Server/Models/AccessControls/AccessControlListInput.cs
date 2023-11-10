using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.AccessControls
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessControlListInput : PageInput
    {
        /// <summary>
        /// Ip地址
        /// </summary>
        public string? Ip { get; set; }
    }
}
