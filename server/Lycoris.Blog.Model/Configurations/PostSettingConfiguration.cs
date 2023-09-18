namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class PostSettingConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Second { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Images { get; set; } = new List<string>();
    }
}
