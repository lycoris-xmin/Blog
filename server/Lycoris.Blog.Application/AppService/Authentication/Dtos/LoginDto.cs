namespace Lycoris.Blog.Application.AppService.Authentication.Dtos
{
    public class LoginDto
    {
        public long Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public bool? IsAdmin { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime TokenExpireTime { get; set; }
    }
}
