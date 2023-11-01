using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "LeaveMessage")]
    public class LeaveMessageQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        private readonly ILycorisLogger _logger;
        private readonly IRepository<LeaveMessage, int> _message;

        public LeaveMessageQueueService(ILycorisLoggerFactory factory, IRepository<LeaveMessage, int> message)
        {
            _logger = factory.CreateLogger<LeaveMessageQueueService>();
            _message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var messageId = data.ToTryInt();
            if (!messageId.HasValue || messageId.Value <= 0)
                return;

            var message = await _message.GetAsync(messageId!.Value);
            if (message == null)
                return;

            var fieIds = new List<Expression<Func<LeaveMessage, object>>>();

            var filter = _message.GetAll().Where(x => x.ParentId == messageId).Where(x => x.Status != LeaveMessageStatusEnum.UserDelete);

            var count = await filter.CountAsync();
            if (count != message.ReplyCount)
            {
                message.ReplyCount = count;
                fieIds.Add(x => x.ReplyCount);
            }

            var redundancy = string.Join(",", await filter.OrderByDescending(x => x.CreateTime).Select(x => x.Id).Take(2).ToListAsync() ?? new List<int>());
            if (message.Redundancy != redundancy)
            {
                message.Redundancy = redundancy;
                fieIds.Add(x => x.Redundancy);
            }

            await _message.UpdateFieIdsAsync(message, fieIds);
        }
    }
}
