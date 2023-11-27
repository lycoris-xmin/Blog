namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveMessageSettingInput
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string>? MessageRemind { get; set; } 

        /// <summary>
        /// 留言频率(单位秒)
        /// </summary>
        public int? FrequencySecond { get; set; } 
    }
}
