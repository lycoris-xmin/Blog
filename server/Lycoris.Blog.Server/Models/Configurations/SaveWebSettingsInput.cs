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
        public DateTime? BuildTime { get; set; }
    }
}
