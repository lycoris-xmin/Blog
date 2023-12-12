namespace Lycoris.Blog.Application.AppServices.Posts.Dtos
{
    public class PostSaveDto : PostInfoDto
    {
        /// <summary>
        /// 是否发布
        /// </summary>
        public new bool? IsPublish { get; set; }
    }
}
