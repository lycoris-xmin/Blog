using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownUploadInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "上传文件不能为空")]
        public IFormFile? File { get; set; }
    }
}
