using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;

namespace Lycoris.Blog.Application.AppServices.LoginTokens.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton, PropertiesAutowired = true)]
    public class LoginTokenAppService : ApplicationBaseService, ILoginTokenAppService
    {
        private const string KEY = "2B9E8A3F";
        private const string IV = "20220101";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiredTime"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GenereateToken(long userId, DateTime expiredTime, bool isAdmin = false)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));
            else if (expiredTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var value = $"{Guid.NewGuid():N}|{userId}|{(isAdmin ? 1 : 0)}|{expiredTime:yyyy-MM-dd HH:mm:ss}|{Guid.NewGuid():N}";

            return SecretHelper.DESEncrypt(SecretHelper.CommonEncrypt(value), KEY, IV);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public (long? userId, DateTime? expiredTime, bool? isAdmin) AnalyzeToken(string token)
        {
            try
            {
                var tmp = SecretHelper.CommonDecrypt(SecretHelper.DESDecrypt(token, KEY, IV) ?? "");

                if (tmp.IsNullOrEmpty())
                    return (null, null, null);

                var tmpSecret = tmp!.Split('|');
                if (tmpSecret == null || tmpSecret.Length != 5)
                    return (null, null, null);

                var userIdStr = tmpSecret[1];
                if (userIdStr.IsNullOrEmpty())
                    return (null, null, null);

                var userId = userIdStr!.ToTryLong();
                if (!userId.HasValue || userId.Value <= 0)
                    return (null, null, null);

                var isAdminStr = tmpSecret[2];
                if (isAdminStr.IsNullOrEmpty())
                    return (null, null, null);

                var isAdmin = isAdminStr == "1";

                var expireTimeStr = tmpSecret[3];
                if (expireTimeStr.IsNullOrEmpty())
                    return (null, null, null);

                var expireTime = expireTimeStr.ToTryDateTime();
                if (!expireTime.HasValue || expireTime.Value == DateTime.MinValue)
                    return (null, null, null);

                return (userId, expireTime, isAdmin);
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
                return (null, null, null);
            }
        }
    }
}
