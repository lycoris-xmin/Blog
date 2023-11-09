namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemSettingsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string ShowDocHost { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public SystemFileClearConfiguration SystemFileClear { get; set; } = new SystemFileClearConfiguration();

        /// <summary>
        /// 
        /// </summary>
        public SystemDBClearConfiguration SystemDBClear { get; set; } = new SystemDBClearConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public class SystemFileClearConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public int StaticFile { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int TempFile { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int LogFile { get; set; } = 7;
    }

    /// <summary>
    /// 
    /// </summary>
    public class SystemDBClearConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public int RequestLog { get; set; } = 30;

        /// <summary>
        /// 
        /// </summary>
        public int BrowseLog { get; set; } = 30;

        /// <summary>
        /// 
        /// </summary>
        public int PostComment { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int LeaveMessage { get; set; } = 0;
    }
}
