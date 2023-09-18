namespace Lycoris.Blog.Application.Cached.AuthenticationCache.Dtos
{
    public class LoginUserCacheDto
    {
        public long Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public bool? GoogleAuthentication { get; set; }

        public bool? IsAdmin { get; set; }

        public DateTime TokenExpireTime { get; set; }
    }
}
