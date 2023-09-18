using Lycoris.Blog.Model.Global.Input;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Chat
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? RoomId { get; set; }
    }
}
