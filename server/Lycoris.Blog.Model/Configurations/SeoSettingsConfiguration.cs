namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SeoSettingsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public BaiduSeoConfiguration Biadu { get; set; } = new BaiduSeoConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public class BaiduSeoConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Site { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Token { get; set; }
    }
}
