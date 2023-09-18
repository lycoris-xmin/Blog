using Lycoris.Blog.Application.AppService.Authentication.Dtos;

namespace Lycoris.Blog.Application.Cached.AuthenticationCache.Dtos
{
    public class LoginOathCodeCacheDto
    {

        public LoginOathCodeCacheDto(string? OathCode, LoginValidateDto? Value)
        {
            this.OathCode = OathCode;
            this.Value = Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? OathCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoginValidateDto? Value { get; set; }
    }
}
