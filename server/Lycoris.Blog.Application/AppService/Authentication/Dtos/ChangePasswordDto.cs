namespace Lycoris.Blog.Application.AppService.Authentication.Dtos
{
    public class ChangePasswordDto
    {
        public string? OldPassword { get; set; }

        public string? Password { get; set; }
    }
}
