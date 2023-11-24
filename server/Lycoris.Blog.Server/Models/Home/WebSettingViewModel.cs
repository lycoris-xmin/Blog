namespace Lycoris.Blog.Server.Models.Home
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSettingViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WebCommonViewModel? Common { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WebOwnerViewModel? Owner { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebCommonViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? WebName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Favicon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ICP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? BuildTime { get; set; }

        /// <summary>
        /// 默认头像
        /// </summary>
        public string? DefaultAvatar { get; set; }
    }
}
