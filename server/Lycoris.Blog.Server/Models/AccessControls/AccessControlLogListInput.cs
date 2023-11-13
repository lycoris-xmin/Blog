using Lycoris.Blog.Model.Global.Input;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.AccessControls
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessControlLogListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? Id { get; set; }
    }
}
