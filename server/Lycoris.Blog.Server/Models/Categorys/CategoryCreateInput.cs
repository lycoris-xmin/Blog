using Lycoris.Blog.Server.PropertyAttribute;
using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Categorys
{
    /// <summary>
    /// 
    /// </summary>
    public class CategoryCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [StringValid("分类名称", Required = true, MinLength = 1, MaxLength = 30)]
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "分类展示图不能为空")]
        public IFormFile? File { get; set; }
    }
}
