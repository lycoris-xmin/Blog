using Lycoris.Blog.Application.Cached.LoginFailedRecord.Models;

namespace Lycoris.Blog.Application.Cached.LoginFailedRecord
{
    public interface ILoginFailedRecordCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        LoginFailedRecordCacheModel? GetLoginFailedRecord(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="data"></param>
        void SetLoginFailedRecord(string email, LoginFailedRecordCacheModel data);
    }
}
