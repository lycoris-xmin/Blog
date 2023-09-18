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
        public string? ShowDocHost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SystemJobSettingsConfiguration? SystemJobSettings { get; set; }
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
