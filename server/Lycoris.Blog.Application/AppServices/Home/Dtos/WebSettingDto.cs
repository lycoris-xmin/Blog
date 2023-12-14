namespace Lycoris.Blog.Application.AppServices.Home.Dtos
{
    public class WebSettingDto
    {
        /// <summary>
        /// 
        /// </summary>
        public WebCommonDto? Common { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WebOwnerDto? Owner { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebCommonDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string? WebName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? WebPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Favicon { get; set; }

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
