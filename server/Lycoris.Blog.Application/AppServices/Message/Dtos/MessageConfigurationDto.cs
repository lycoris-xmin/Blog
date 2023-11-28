namespace Lycoris.Blog.Application.AppServices.Message.Dtos
{
    public class MessageConfigurationDto
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
