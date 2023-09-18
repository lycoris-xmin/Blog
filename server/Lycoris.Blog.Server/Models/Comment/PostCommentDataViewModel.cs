﻿namespace Lycoris.Blog.Server.Models.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class PostCommentDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BlogCommentUserInfoViewModel? User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RepliedUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RepliedUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? AgentFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOwner { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BlogCommentUserInfoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Avatar { get; set; }
    }
}
