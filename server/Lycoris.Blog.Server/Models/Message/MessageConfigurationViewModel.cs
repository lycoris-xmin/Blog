namespace Lycoris.Blog.Server.Models.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageConfigurationViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string>? MessageRemind { get; set; }

        /// <summary>
        /// 留言频率(单位秒)
        /// </summary>
        public int FrequencySecond { get; set; }
    }
}
