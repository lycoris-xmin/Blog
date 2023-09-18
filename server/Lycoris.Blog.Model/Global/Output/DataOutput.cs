namespace Lycoris.Blog.Model.Global.Output
{
    /// <summary>
    /// 内容响应体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataOutput<T> : BaseOutput where T : class
    {
        /// <summary>
        /// 响应内容
        /// </summary>
        public T? Data { get; set; }
    }
}
