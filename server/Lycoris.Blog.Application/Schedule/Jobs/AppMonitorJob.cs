using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Application.SignalR.Hubs;
using Lycoris.Blog.Application.SignalR.Models;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Lycoris.Quartz.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    [QuartzJob("服务监控", Trigger = QuartzTriggerEnum.SIMPLE, IntervalSecond = 5)]
    public class AppMonitorJob : BaseJob
    {
        private readonly AppMonitorContext _monitorContext;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly Lazy<IRepository<ServerMonitor, int>> _serverMonitor;
        private readonly Lazy<IRepository<RequestLog, long>> _requestLog;
        private readonly Lazy<IRepository<BrowseLog, long>> _browseLog;
        private readonly Lazy<IRepository<WebDayStatistics, DateTime>> _webDayStatistics;
        private readonly Lazy<IRepository<PostComment, long>> _postComment;
        private readonly Lazy<IRepository<LeaveMessage, int>> _leaveMessage;


        public AppMonitorJob(ILycorisLoggerFactory factory,
                                AppMonitorContext monitorContext,
                                IHubContext<DashboardHub> hubContext,
                                Lazy<IRepository<ServerMonitor, int>> serverMonitor,
                                Lazy<IRepository<RequestLog, long>> requestLog,
                                Lazy<IRepository<BrowseLog, long>> browseLog,
                                Lazy<IRepository<WebDayStatistics, DateTime>> webDayStatistics,
                                Lazy<IRepository<PostComment, long>> postComment,
                                Lazy<IRepository<LeaveMessage, int>> leaveMessage) : base(factory.CreateLogger<AppMonitorJob>())
        {
            _monitorContext = monitorContext;
            _monitorContext.Server.MnitorCount ??= 0;
            _hubContext = hubContext;
            _serverMonitor = serverMonitor;
            _requestLog = requestLog;
            _browseLog = browseLog;
            _webDayStatistics = webDayStatistics;
            _postComment = postComment;
            _leaveMessage = leaveMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            // 服务器性能监控
            var serverMonitor = await ServerMonitorHandlerAsync();
            var requestMonitor = await RequestMonitorHandlerAsync();

            if (_monitorContext.ServerMonitorConnectionIds.HasValue())
            {
                if (serverMonitor.HasValue())
                    await _hubContext.Clients.Group(DashboardHub.ServerMonitorGroup).SendAsync("ServerMonitor", serverMonitor);

                if (requestMonitor.HasValue())
                    await _hubContext.Clients.Group(DashboardHub.ServerMonitorGroup).SendAsync("RequestMonitor", requestMonitor);
            }

            var hourStatisticsMonitor = await WebToDayHourStatisticsHandlerAsync();
            if (hourStatisticsMonitor != null && _monitorContext.HourStatisticsConnectionIds.HasValue())
                await _hubContext.Clients.Group(DashboardHub.HourStatisticsMonitorGroup).SendAsync("HourStatisticsMonitor", hourStatisticsMonitor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<List<ServerMonitorModel>> ServerMonitorHandlerAsync()
        {
            var info = ComputerHelper.GetComputerInfo();
            _monitorContext.Server.TotalRAM = info.TotalRAM;

            var time = DateTime.Now;
            time = time.AddSeconds(-(time.Second % 5));

            _monitorContext.Server.List.Add(new ServerMonitorDetail()
            {
                MonitorTime = time,
                CPURate = info.CPURate,
                RAMRate = info.RAMRate * 100
            });

            while (_monitorContext.Server.List.Count < 10)
            {
                _monitorContext.Server.List.Insert(0, new ServerMonitorDetail()
                {
                    MonitorTime = _monitorContext.Server.List.Min(x => x.MonitorTime).AddSeconds(-5),
                    CPURate = 0,
                    RAMRate = 0
                });
            }

            if (_monitorContext.Server.List.Count > 10)
                _monitorContext.Server.List.RemoveAt(0);

            if (_monitorContext.Server.MnitorCount >= 10)
            {
                var data = await _serverMonitor.Value.GetAll().Where(x => x.Time == DateTime.Now.Date).SingleOrDefaultAsync() ?? new ServerMonitor() { Time = DateTime.Now.Date };

                // cpu
                var maxCpuRate = (int)Math.Ceiling(_monitorContext.Server.List.Average(x => x.CPURate) * 100);
                // ram
                var maxRamRate = (int)Math.Ceiling(_monitorContext.Server.List.Average(x => x.RAMRate) * 100);

                var fieIds = new List<Expression<Func<ServerMonitor, object>>>();

                data.UpdatePorpertyIf(data.MaxCPURate < maxCpuRate, x =>
                {
                    x.MaxCPURate = maxCpuRate;
                    fieIds.Add(x => x.MaxCPURate);
                }).UpdatePorpertyIf(data.MaxRAMRate < maxCpuRate, x =>
                {
                    x.MaxRAMRate = maxRamRate;
                    fieIds.Add(x => x.MaxRAMRate);
                });

                await _serverMonitor.Value.CreateOrUpdateAsync(data, fieIds);

                _monitorContext.Server.MnitorCount = 0;
            }
            else
                _monitorContext.Server.MnitorCount++;

            return _monitorContext.Server.List.Select(x => new ServerMonitorModel()
            {
                MonitorTime = x.MonitorTime,
                CPURate = x.CPURate.ToString("0.00"),
                RAMRate = x.RAMRate.ToString("0.00")
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<List<RequestMonitorModel>?> RequestMonitorHandlerAsync()
        {
            var time = DateTime.Now;
            time = time.AddMinutes(-(time.Minute % 5));

            if (_monitorContext.Request.HasValue() && _monitorContext.Request.Max(x => x.MonitorTime).AddMinutes(5) >= time)
                return _monitorContext.Request.ToMapList<RequestMonitorModel>();

            var data = new RequestMonitorContext() { MonitorTime = time };

            var startTime = _monitorContext.Request.Count > 0 ? _monitorContext.Request.Max(x => x.MonitorTime) : time.AddMinutes(-5);

            data.Request = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= startTime && x.CreateTime <= time).CountAsync();
            data.Success = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= startTime && x.CreateTime <= time).Where(x => x.Success == true).CountAsync();
            data.Error = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= startTime && x.CreateTime <= time).Where(x => x.Success == false).CountAsync();
            data.PV = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= startTime && x.CreateTime <= time).CountAsync();
            data.UV = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= startTime && x.CreateTime <= time).GroupBy(x => x.ClientOrign).Select(x => 1).SumAsync(x => x);

            _monitorContext.Request.Add(data);

            while (_monitorContext.Request.Count < 10)
            {
                _monitorContext.Request.Insert(0, new RequestMonitorContext
                {
                    MonitorTime = _monitorContext.Request.Min(x => x.MonitorTime).AddMinutes(-5),
                    Request = 0,
                    Success = 0,
                    Error = 0,
                    PV = 0,
                    UV = 0
                });
            }

            if (_monitorContext.Request.Count > 10)
                _monitorContext.Request.RemoveAt(0);

            return _monitorContext.Request.ToMapList<RequestMonitorModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<HourStatisticsSignalRModel?> WebToDayHourStatisticsHandlerAsync()
        {
            try
            {
                var startTime = _monitorContext.HourStatistics.LastTime ?? DateTime.Now.AddMinutes(-5);

                if (startTime.AddMinutes(5) > DateTime.Now)
                    return _monitorContext.HourStatistics.ToMap<HourStatisticsSignalRModel>();

                var endTime = startTime.AddMinutes(5);
                var yesterday = DateTime.Now.Date.AddDays(-1);
                var today = DateTime.Now.Date;

                var yesterdayStatistics = this.Context.GetJobDataMap<WebDayStatistics>("WebDayStatistics");
                if (yesterdayStatistics == null || yesterdayStatistics.Id != yesterday)
                {
                    yesterdayStatistics = await _webDayStatistics.Value.GetAsync(yesterday) ?? new WebDayStatistics() { Id = DateTime.MinValue };

                    if (yesterdayStatistics.Id != DateTime.MinValue)
                        this.Context.AddJobDataMap("WebDayStatistics", yesterdayStatistics);
                }

                // 浏览量计算
                {
                    _monitorContext.HourStatistics.PVBrowse = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync();
                    // 与昨日计算百分比
                    _monitorContext.HourStatistics.PVBrowsePercent = CalcHourStatisticsPercent(yesterdayStatistics.PVBrowse, _monitorContext.HourStatistics.PVBrowse);
                }


                // 访客计算
                {
                    _monitorContext.HourStatistics.OnlineUsers = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= startTime && x.CreateTime < endTime).GroupBy(x => x.ClientOrign).CountAsync();
                    _monitorContext.HourStatistics.UVBrowse = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= today).GroupBy(x => x.ClientOrign).CountAsync();
                    // 与昨日计算百分比
                    _monitorContext.HourStatistics.UVBrowsePercent = CalcHourStatisticsPercent(yesterdayStatistics.UVBrowse, _monitorContext.HourStatistics.UVBrowse);
                }


                // 评论、留言计算
                {
                    _monitorContext.HourStatistics.CommentMessage = await _postComment.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync();
                    // 与昨日计算百分比
                    _monitorContext.HourStatistics.CommentMessagePercent = CalcHourStatisticsPercent(yesterdayStatistics.CommentMessage, _monitorContext.HourStatistics.PVBrowse);
                }


                // 平均响应耗时
                {
                    var requestStartTime = DateTime.Now.Date;
                    var requestCount = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= requestStartTime && x.CreateTime < endTime).CountAsync();
                    if (requestCount > 0)
                    {
                        var totalElapsedMilliseconds = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= requestStartTime && x.CreateTime < endTime).SumAsync(x => x.ElapsedMilliseconds);

                        var oldElapsedMilliseconds = _monitorContext.HourStatistics.ElapsedMilliseconds;
                        _monitorContext.HourStatistics.ElapsedMilliseconds = (int)Math.Ceiling((double)totalElapsedMilliseconds / requestCount);
                        _monitorContext.HourStatistics.ElapsedMillisecondsDifference = oldElapsedMilliseconds > 0 ? oldElapsedMilliseconds - _monitorContext.HourStatistics.ElapsedMilliseconds : 0;
                    }
                }

                _monitorContext.HourStatistics.LastTime = endTime;

                return _monitorContext.HourStatistics.ToMap<HourStatisticsSignalRModel>();
            }
            catch (Exception ex)
            {
                this.JobLogger.Error("handle web today hour statistics failed", ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="old"></param>
        /// <param name="new"></param>
        /// <returns></returns>
        private static double CalcHourStatisticsPercent(int old, int @new)
        {
            if (old == 0 && @new == 0)
                return 0d;
            else if (old == 0)
                return 100d;
            else if (@new == 0)
                return -100d;

            var val = @new - old;
            var percent = Math.Ceiling((Math.Abs(val) / (double)old) * 10000d) / 100;
            return val > 0 ? percent : -percent;
        }
    }
}
