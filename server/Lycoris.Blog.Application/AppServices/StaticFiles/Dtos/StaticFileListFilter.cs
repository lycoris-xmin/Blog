using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Application.AppServices.StaticFiles.Dtos
{
    public class StaticFileListFilter : PageFilter
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public FileUploadChannelEnum? UploadChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? LocalBack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Use { get; set; }
    }
}
