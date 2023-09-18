using Lycoris.Blog.Application.AppService.LoginFailedRecords.Dtos;

namespace Lycoris.Blog.Application.Cached.LoginFailedRecordCache
{
    public interface ILoginFailedRecordCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        Task<LoginFailedRecordDto?> GetLoginFailedRecordAsync(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="data"></param>
        Task SetLoginFailedRecordAsync(string email, LoginFailedRecordDto data);
    }
}
