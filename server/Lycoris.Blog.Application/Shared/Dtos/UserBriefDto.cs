namespace Lycoris.Blog.Application.Shared.Dtos
{
    public class UserBriefDto
    {
        public long? Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public string? Email { get; set; }

        public string? Blog { get; set; }

        public string? Github { get; set; }

        public string? Gitee { get; set; }

        public string? QQ { get; set; }

        public string? WeChat { get; set; }

        public string? CloudMusic { get; set; }

        public string? Bilibili { get; set; }

        public bool IsAdmin { get; set; }
    }
}
