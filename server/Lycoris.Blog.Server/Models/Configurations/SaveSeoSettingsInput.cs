namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveSeoSettingsInput
    {
        /// <summary>
        /// 
        /// </summary>
        public BaiduSeoSettings? Biadu { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BaiduSeoSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? Enabled { get; set; }

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
