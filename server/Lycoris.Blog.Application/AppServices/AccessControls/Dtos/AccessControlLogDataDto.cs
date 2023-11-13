namespace Lycoris.Blog.Application.AppServices.AccessControls.Dtos
{
    public class AccessControlLogDataDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 请求路由
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string? Params { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public uint StatusCode { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        public string? Response { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
