using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SavePostSettingsInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool? AutoSave { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Second { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string>? Images { get; set; }
    }
}
