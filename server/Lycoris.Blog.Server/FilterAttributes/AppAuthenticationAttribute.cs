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
    /// App端身份验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AppAuthenticationAttribute : BaseActionAttribute
    {
        /// <summary>
        /// 是否必须验证登录信息
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ActionHandlerBeforeAsync(ActionExecutingContext context)
        {
            if (CheckAllowAnonymous(context.ActionDescriptor))
                return;

            var request = context.HttpContext.GetService<RequestContext>();

            if (request.Token.IsNullOrEmpty())
                throw new HttpStatusException(HttpStatusCode.Unauthorized, "can not find authentication token in request context");

            var _authentication = context.GetService<IAuthenticationAppService>();

            var data = await _authentication.GetLoginUserAsync(request.Token, true) ?? throw new HttpStatusException(HttpStatusCode.Unauthorized, "can not find authentication token in request context");

            request.User = new RequestUserContext()
            {
                Id = data.Id,
                NickName = data.NickName ?? "",
                Avatar = data.Avatar ?? "",
                IsAdmin = data.IsAdmin ?? false
            };

            request.TokenExpireTime = data.TokenExpireTime;

            await Task.CompletedTask;
        }
    }
}
