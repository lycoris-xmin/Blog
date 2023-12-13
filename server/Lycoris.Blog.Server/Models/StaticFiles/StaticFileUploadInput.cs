using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.StaticFiles
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticFileUploadInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public IFormFile? File { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public UploadType? UploadType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum UploadType
    {
        /// <summary>
        /// 文章封面图
        /// </summary>
        PostIcon = 0,
        /// <summary>
        /// 文章轮播图
        /// </summary>
        PostCarousel = 1,
        /// <summary>
        /// 文章文件
        /// </summary>
        Post = 2,
        /// <summary>
        /// 文章分类图
        /// </summary>
        CategoryIcon = 10,
        /// <summary>
        /// 关于
        /// </summary>
        AboutWeb = 20,
        /// <summary>
        /// 
        /// </summary>
        Logo = 30,
        /// <summary>
        /// 
        /// </summary>
        Avatar = 40,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 99
    }
}
