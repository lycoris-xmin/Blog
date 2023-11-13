using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Users.Dtos
{
    public class GetUserListFilter : PageFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Email { get; set; }
    }
}
