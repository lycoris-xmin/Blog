namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveWebSettingsInput
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
        public string? AdminPath { get; set; }

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
        /// 
        /// </summary>
        public string? LogoDisplay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IFormFile? Logo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IFormFile? Avatar { get; set; }
    }
}
