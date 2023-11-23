using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.EntityFrameworkCore.Migrations;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "RequestLog")]
    public class RequestLogQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        public JobLogger? JobLogger { get; set; }

        private readonly IRepository<RequestLog, long> _requestLog;
        private readonly IRepository<AccessControl, int> _accessControl;
        private readonly IRepository<AccessControlLog, long> _accessControlLog;

        public RequestLogQueueService(IRepository<RequestLog, long> requestLog,
                                      IRepository<AccessControl, int> accessControl,
                                      IRepository<AccessControlLog, long> accessControlLog)
        {
            _requestLog = requestLog;
            _accessControl = accessControl;
            _accessControlLog = accessControlLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var model = data.ToObject<RequestLogQueueModel>();
            if (model == null)
            {
                this.JobLogger!.Error("can not find any data");
                return;
            }

            var log = model!.ToMap<RequestLog>();

            if (!model!.Ip.IsNullOrEmpty())
            {
                log.Ip = IPAddressHelper.Ipv4ToUInt32(model.Ip);

                if (IPAddressHelper.IsPrivateNetwork(model.Ip))
                    log.IpAddress = "局域网";
                else
                    log.IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(model.Ip));
            }
            else
                log.IpAddress = "未知";

            log.Success = log.StatusCode == 200;

            if (log.Success)
            {
                var resp = log.Response.ToObject<BaseOutput>();
                log.Success = resp != null && (resp.ResCode == ResCodeEnum.Success || resp.ResCode == ResCodeEnum.TokenExpired);
            }

            log = await _requestLog.CreateAsync(log);

            await DealAccessControlAsync(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private async Task DealAccessControlAsync(RequestLog log)
        {
            var data = await _accessControl.GetAll().Where(x => x.Ip == log.Ip).SingleOrDefaultAsync();
            if (data == null)
                return;

            data.Count++;
            data.LastAccessTime = DateTime.Now;

            // 更新数据
            await _accessControl.UpdateFieIdsAsync(data, x => x.Count, x => x.LastAccessTime);

            var accessLog = log.ToMap<AccessControlLog>();
            accessLog.AccessControlId = data.Id;
            // 插入管控日志记录
            await _accessControlLog.CreateAsync(accessLog);
        }
    }
}
