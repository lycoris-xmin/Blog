using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "LoginRecord")]
    public class LoginRecordQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        private readonly IRepository<LoginRecord, int> _LoginRecord;

        public LoginRecordQueueService(IRepository<LoginRecord, int> LoginRecord)
        {
            _LoginRecord = LoginRecord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var dto = data?.ToObject<LoginRecordQueueDto>();
            if (dto == null)
                return;

            var record = await _LoginRecord.GetAll().Where(x => x.UserId == dto.UserId).FirstOrDefaultAsync();

            var tmp = new LoginRecord()
            {
                UserId = dto.UserId,
                UserAgent = dto.UserAgent,
                Ip = IPAddressHelper.Ipv4ToUInt32(dto.Ip),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(dto.Ip)),
                LoginTime = time ?? DateTime.Now
            };

            if (record != null && record.Ip == tmp.Ip && record.UserAgent == dto.UserAgent && record.LoginTime.AddMinutes(10) > DateTime.Now)
                return;

            await _LoginRecord.CreateAsync(tmp);
        }
    }
}
