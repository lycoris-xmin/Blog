using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Talks
{
    /// <summary>
    /// 
    /// </summary>
    public class MasterTalkCreateOrUpdateInput
    {
        /// <summary>
        /// 
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Content { get; set; }
    }
}
