using Lycoris.Blog.Model.Global.Input;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Talks
{
    /// <summary>
    /// 
    /// </summary>
    public class TalkCommentListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? TalkId { get; set; }
    }
}
