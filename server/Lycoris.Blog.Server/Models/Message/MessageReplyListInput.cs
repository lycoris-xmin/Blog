﻿using Lycoris.Blog.Model.Global.Input;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageReplyListInput : PageInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? MessageId { get; set; }
    }
}
