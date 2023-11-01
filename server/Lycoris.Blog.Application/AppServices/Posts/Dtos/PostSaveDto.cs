using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostSaveDto : PostInfoDto
    {
        /// <summary>
        /// 简介
        /// </summary>
        public string? Info { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        public new bool? IsPublish { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Recommend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IFormFile? File { get; set; }
    }
}
