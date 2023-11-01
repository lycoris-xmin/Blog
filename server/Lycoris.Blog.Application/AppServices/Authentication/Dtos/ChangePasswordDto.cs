namespace Lycoris.Blog.Application.AppServices.Authentication.Dtos
{
    public class ChangePasswordDto
    {
        public string? OldPassword { get; set; }

        public string? Password { get; set; }
    }
}
