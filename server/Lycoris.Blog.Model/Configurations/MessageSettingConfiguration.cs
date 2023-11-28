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
        public List<string> MessageRemind { get; set; } = new List<string>()
        {
           "请理性发言，不造谣、不传谣，不诋毁谩骂、恶意攻击他人、不要发布有关国家政治、国家统一等相关的留言。",
           "请不要随意发布类似水贴、广告等相关留言或攻击本网站的言论。",
           "请不要相信出现在留言中的任何广告推销、技术培训、有偿解决问题等相关信息。",
           "博主会定期审核留言、不当言论会删除、由于有其他本职工作，删除可能并不是那么及时，若影响各位浏览，尽请谅解。"
        };

        /// <summary>
        /// 留言频率(单位秒)
        /// </summary>
        public int FrequencySecond { get; set; } = 10;
    }
}
