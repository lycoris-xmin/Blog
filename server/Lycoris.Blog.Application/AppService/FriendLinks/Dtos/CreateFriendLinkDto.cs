namespace Lycoris.Blog.Application.AppService.FriendLinks.Dtos
{
    public class CreateFriendLinkDto
    {
        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
