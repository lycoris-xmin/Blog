namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageSettingConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> MessageRemind { get; set; } = new List<string>();

        /// <summary>
        /// 留言频率(单位秒)
        /// </summary>
        public int FrequencySecond { get; set; } = 0;
    }
}
