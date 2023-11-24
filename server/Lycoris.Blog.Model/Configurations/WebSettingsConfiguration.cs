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
        public string WebName { get; set; } = "程序猿的小破站";

        /// <summary>
        /// 
        /// </summary>
        public string WebPath { get; set; } = "http://127.0.0.1:5173";

        /// <summary>
        /// 
        /// </summary>
        public string AdminPath { get; set; } = "http://127.0.0.1:5174";

        /// <summary>
        /// 
        /// </summary>
        public string Logo { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string ICP { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public DateTime BuildTime { get; set; } = new DateTime(2000, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        public string DefaultAvatar { get; set; } = string.Empty;
    }
}
