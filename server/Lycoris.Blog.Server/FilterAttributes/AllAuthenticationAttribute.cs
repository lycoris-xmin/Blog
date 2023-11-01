using Lycoris.Blog.Application.AppServices.Authentication;
using Lycoris.Blog.Common.Extensions;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Server.Shared;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Lycoris.Blog.Server.FilterAttributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AllAuthenticationAttribute : BaseActionAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ActionHandlerBeforeAsync(ActionExecutingContext context)
        {
            if (CheckAllowAnonymous(context.ActionDescriptor) || CheckCustomeAttribute<AppAuthenticationAttribute>(context.ActionDescriptor) || CheckCustomeAttribute<WebAuthenticationAttribute>(context.ActionDescriptor))
                return;

            var currentReuqest = context.HttpContext.GetService<RequestContext>();
            if (currentReuqest.Token.IsNullOrEmpty())
            {
                if (IsRequired)
                    throw new HttpStatusException(HttpStatusCode.Unauthorized, "no usable access token found");

                return;
            }

            var authentication = context.HttpContext.GetService<IAuthenticationAppService>();
            var cache = await authentication.GetLoginUserAsync(currentReuqest.Token, false);
            cache ??= await authentication.GetLoginUserAsync(currentReuqest.Token, true);

            if (cache == null)
            {
                if (IsRequired)
                    throw new HttpStatusException(HttpStatusCode.Unauthorized, $"token:{currentReuqest.Token} analyze failed");

                return;
            }

            currentReuqest.User = new RequestUserContext()
            {
                Id = cache.Id,
                NickName = cache.NickName ?? "",
                IsAdmin = cache.IsAdmin ?? false
            };

            currentReuqest.TokenExpireTime = cache.TokenExpireTime;
        }
    }
}
