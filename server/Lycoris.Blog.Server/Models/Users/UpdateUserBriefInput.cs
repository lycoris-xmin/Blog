namespace Lycoris.Blog.Server.Models.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserBriefInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Blog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Github { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Gitee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? QQ { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Wechat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CloudMusic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Bilibili { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IFormFile? File { get; set; }
    }
}
