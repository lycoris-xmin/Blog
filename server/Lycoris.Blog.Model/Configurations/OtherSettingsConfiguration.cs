namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class OtherSettingsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public ShowdocSettingsConfiguration Showdoc { get; set; } = new ShowdocSettingsConfiguration();

        /// <summary>
        /// 
        /// </summary>
        public DataClearConfiguration DataClear { get; set; } = new DataClearConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShowdocSettingsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool MonitoringPush { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public int CPURate { get; set; } = 50;

        /// <summary>
        /// 
        /// </summary>
        public int RAMRate { get; set; } = 50;

        /// <summary>
        /// 
        /// </summary>
        public bool CommentPush { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool MessagePush { get; set; } = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataClearConfiguration
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
