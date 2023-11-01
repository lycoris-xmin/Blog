using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.AppServices.LoginFailedRecords;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "LoginFailedRecord")]
    public class LoginFailedRecordQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        private readonly ILoginFailedRecordService _loginFailedRecord;

        public LoginFailedRecordQueueService(ILoginFailedRecordService loginFailedRecord)
        {
            _loginFailedRecord = loginFailedRecord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var dto = data?.ToObject<LoginFailedRecordQueueModel>();
            if (dto == null)
                return;

            var record = await _loginFailedRecord.GetAll().Where(x => x.Email == dto.Email).FirstOrDefaultAsync() ?? new LoginFailedRecord() { Email = dto.Email };

            if (record.Count >= dto.Count && record.FreezeTime > DateTime.Now)
                return;

            record.Count = dto.Count;

            if (record.Count >= 5)
            {
                record.FreezeTime ??= DateTime.Now.AddMinutes(30);
                if (record.FreezeTime.Value <= DateTime.Now)
                    record.FreezeTime = DateTime.Now.AddMinutes(30);

                if (record.FreezeTime.Value.Second > 0)
                    record.FreezeTime = record.FreezeTime.Value.AddSeconds(60 - record.FreezeTime.Value.Second);
            }
            else
                record.FreezeTime = null;

            await _loginFailedRecord.SetLoginFailedRecordAsync(record);
        }
    }
}
