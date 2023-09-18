using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleIdArrayInput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public T[]? Ids { get; set; }
    }
}
