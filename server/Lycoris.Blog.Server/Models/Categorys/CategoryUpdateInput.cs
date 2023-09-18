using Lycoris.Blog.Server.PropertyAttribute;

namespace Lycoris.Blog.Server.Models.Categorys
{
    /// <summary>
    /// 
    /// </summary>
    public class CategoryUpdateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [StringValid("分类编号", Required = true)]
        public string? Id { get; set; }

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
        public IFormFile? File { get; set; }
    }
}
