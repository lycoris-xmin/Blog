namespace Lycoris.Blog.Application.AppServices.Authentication.Dtos
{
    public class LoginValidateDto
    {
        public LoginValidateDto() { }

        public LoginValidateDto(long Id)
        {
            this.Id = Id;
        }

        public long? Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public bool? IsAdmin { get; set; }

        public bool? GoogleAuthentication { get; set; }
    }
}
