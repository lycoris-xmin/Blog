using Lycoris.Blog.Model.Global.Input;

namespace Lycoris.Blog.Server.Models.StaticFiles
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticFileRepositoryInput : PageInput
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        public int? FileType { get; set; }
    }
}
