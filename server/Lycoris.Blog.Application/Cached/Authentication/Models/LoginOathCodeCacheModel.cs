using Lycoris.Blog.Application.AppServices.Authentication.Dtos;

namespace Lycoris.Blog.Cache.Authentication.Models
{
    public class LoginOathCodeCacheModel
    {
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
