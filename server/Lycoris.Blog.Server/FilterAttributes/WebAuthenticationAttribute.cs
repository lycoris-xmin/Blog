using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.Authentication;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Server.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Lycoris.Blog.Server.FilterAttributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class WebAuthenticationAttribute : BaseActionAttribute
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
            if (CheckAllowAnonymous(context.ActionDescriptor) || CheckCustomeAttribute<AppAuthenticationAttribute>(context.ActionDescriptor))
                return;

            var currentReuqest = context.HttpContext.GetService<RequestContext>();
            if (currentReuqest.Token.IsNullOrEmpty())
            {
                if (IsRequired)
                    throw new HttpStatusException(HttpStatusCode.Unauthorized, "no usable access token found");

                return;
            }

            var authentication = context.HttpContext.GetService<IAuthenticationAppService>();

            var data = await authentication.GetLoginUserAsync(currentReuqest.Token, false);
            if (data == null)
            {
                if (IsRequired)
                    throw new HttpStatusException(HttpStatusCode.Unauthorized, $"token:{currentReuqest.Token} analyze failed");

                return;
            }

            currentReuqest.User = new RequestUserContext()
            {
                Id = data.Id,
                NickName = data.NickName ?? "",
                Avatar = data.Avatar ?? "",
                IsAdmin = data.IsAdmin ?? false
            };

            currentReuqest.TokenExpireTime = data.TokenExpireTime;
        }
    }
}
