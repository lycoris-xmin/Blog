using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Global.Output;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "RequestLog")]
    public class RequestLogQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        private readonly ILycorisLogger _logger;
        private readonly IRepository<RequestLog, long> _requestLog;

        public RequestLogQueueService(ILycorisLoggerFactory factory, IRepository<RequestLog, long> requestLog)
        {
            _logger = factory.CreateLogger<RequestLogQueueService>();
            _requestLog = requestLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var dto = data.ToObject<RequestLogQueueDto>();
            if (dto == null)
                return;

            var log = dto!.ToMap<RequestLog>();

            if (!dto!.IP.IsNullOrEmpty())
            {
                log.IP = IPAddressHelper.Ipv4ToUInt32(dto.IP);

                if (IPAddressHelper.IsPrivateNetwork(dto.IP))
                    log.IPAddress = "局域网";
                else
                    log.IPAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(dto.IP));
            }
            else
                log.IPAddress = "未知";

            log.Success = log.StatusCode == 200;

            if (log.Success)
            {
                var resp = log.Response.ToObject<BaseOutput>();
                log.Success = resp != null && (resp.ResCode == ResCodeEnum.Success || resp.ResCode == ResCodeEnum.TokenExpired);
            }

            await _requestLog.CreateAsync(log);
        }
    }
}
