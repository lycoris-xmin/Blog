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
        public string ShowDocHost { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public SystemJobSettingsConfiguration SystemJobSettings { get; set; } = new SystemJobSettingsConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public class SystemJobSettingsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public int[]? RequestLogInCludeStatusCode { get; set; }
    }
}
