using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Server.Models.StaticFiles
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticFileListInput : PageFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? SaveChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Use { get; set; }
    }
}
