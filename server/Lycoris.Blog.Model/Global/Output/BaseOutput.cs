namespace Lycoris.Blog.Model.Global.Output
{
    /// <summary>
    /// 基础响应体
    /// </summary>
    public class BaseOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public ResCodeEnum ResCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResMsg { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string? TraceId { get; set; }
    }

    /// <summary>
    /// 响应码枚举
    /// </summary>
    public enum ResCodeEnum
    {
        /// <summary>
        /// 程序错误
        /// </summary>
        ModelStateError = -1099,

        /// <summary>
        /// 程序错误
        /// </summary>
        ApplicationError = -999,

        /// <summary>
        /// 聊天消息发送失败
        /// </summary>
        ChatPuhlishFailed = -777,

        /// <summary>
        /// 友好提示
        /// </summary>
        Friendly = -99,

        /// <summary>
        /// 访问令牌过期
        /// </summary>
        TokenExpired = -21,

        /// <summary>
        /// 数据找不到
        /// </summary>
        DataNotFound = -1,

        /// <summary>
        /// 请求成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 文章还未发布
        /// </summary>
        ArticleNotPublish = 10,

        /// <summary>
        /// 发布失败
        /// </summary>
        IMPublishFailed = 11,

        /// <summary>
        /// 远端文件已存在
        /// </summary>
        RemoteFileRepeat = 110,
    }
}
