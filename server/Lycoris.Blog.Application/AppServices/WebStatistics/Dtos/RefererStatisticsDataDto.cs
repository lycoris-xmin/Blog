namespace Lycoris.Blog.Application.AppServices.WebStatistics.Dtos
{
    public class RefererStatisticsDataDto
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string? Referer { get; set; }

        /// <summary>
        /// 来源域名
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Count { get; set; }
    }
}
