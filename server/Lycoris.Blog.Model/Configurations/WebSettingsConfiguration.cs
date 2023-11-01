namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSettingsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string WebName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string WebPath { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string AdminPath { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public DateTime BuildTime { get; set; } = new DateTime(2000, 1, 1);
    }
}
