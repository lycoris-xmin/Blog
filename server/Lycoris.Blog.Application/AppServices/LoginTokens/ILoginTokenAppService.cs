using Lycoris.Blog.Application.Shared;

namespace Lycoris.Blog.Application.AppServices.LoginTokens
{
    public interface ILoginTokenAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        string GenereateToken(long userId, DateTime expiredTime, bool isAdmin = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        (long? userId, DateTime? expiredTime, bool? isAdmin) AnalyzeToken(string token);
    }
}
