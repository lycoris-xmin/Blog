namespace Lycoris.Blog.Server.Models.Categorys
{
    /// <summary>
    /// 
    /// </summary>
    public class CategoryDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string>? Keyword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? PostCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
