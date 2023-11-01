namespace Lycoris.Blog.Application.AppServices.FriendLinks.Dtos
{
    public class FriendLinkApplyDto
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 头像链接
        /// </summary>
        public string Icon { get; set; } = "";

        /// <summary>
        /// 网站链接
        /// </summary>
        public string Link { get; set; } = "";

        /// <summary>
        /// 网站介绍
        /// </summary>
        public string Description { get; set; } = "";
    }
}
