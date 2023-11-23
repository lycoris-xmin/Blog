using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "LoginRecord")]
    public class LoginRecordQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        public JobLogger? JobLogger { get; set; }

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
            var model = data?.ToObject<LoginRecordQueueModel>();
            if (model == null)
            {
                this.JobLogger!.Error("can not find any data");
                return;
            }

            var record = await _LoginRecord.GetAll().Where(x => x.UserId == model.UserId).FirstOrDefaultAsync();

            var tmp = new LoginRecord()
            {
                UserId = model.UserId,
                UserAgent = model.UserAgent,
                Ip = IPAddressHelper.Ipv4ToUInt32(model.Ip),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(model.Ip)),
                LoginTime = time ?? DateTime.Now
            };

            if (record != null && record.Ip == tmp.Ip && record.UserAgent == model.UserAgent && record.LoginTime.AddMinutes(10) > DateTime.Now)
                return;

            await _LoginRecord.CreateAsync(tmp);
        }
    }
}
