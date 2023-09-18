using Lycoris.Blog.Application.AppService.LoginFailedRecords.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Tables;

namespace Lycoris.Blog.Application.AppService.LoginFailedRecords
{
    public interface ILoginFailedRecordService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IQueryable<LoginFailedRecord> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<LoginFailedRecordDto> GetLoginFailedRecordAsync(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        Task SetLoginFailedRecordAsync(LoginFailedRecord record);
    }
}
