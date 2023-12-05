using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application.AppServices.WebStatistics.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppServices.WebStatistics.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class WebStatisticsAppService : ApplicationBaseService, IWebStatisticsAppService
    {
        private readonly IRepository<WebDayStatistics, DateTime> _webDayStatistics;
        private readonly IRepository<BrowseStatistics, int> _browseStatistics;
        private readonly IRepository<RefererStatistics, int> _refererStatistics;
        private readonly IRepository<BrowseWorldMap, int> _browseWorldMap;
        private readonly Lazy<AppMonitorContext> _monitorContext;
        private readonly Lazy<IRepository<RequestLog, long>> _requestLog;
        private readonly Lazy<IRepository<BrowseLog, long>> _browseLog;
        private readonly Lazy<IRepository<PostComment, long>> _postComment;
        private readonly Lazy<IRepository<LeaveMessage, int>> _leaveMessage;

        public WebStatisticsAppService(IRepository<WebDayStatistics, DateTime> webDayStatistics,
                                       IRepository<BrowseStatistics, int> browseStatistics,
                                       IRepository<RefererStatistics, int> refererStatistics,
                                       IRepository<BrowseWorldMap, int> browseWorldMap,
                                       Lazy<AppMonitorContext> monitorContext,
                                       Lazy<IRepository<RequestLog, long>> requestLog,
                                       Lazy<IRepository<BrowseLog, long>> browseLog,
                                       Lazy<IRepository<PostComment, long>> postComment,
                                       Lazy<IRepository<LeaveMessage, int>> leaveMessage)
        {
            _webDayStatistics = webDayStatistics;
            _browseStatistics = browseStatistics;
            _refererStatistics = refererStatistics;
            _browseWorldMap = browseWorldMap;
            _monitorContext = monitorContext;
            _requestLog = requestLog;
            _browseLog = browseLog;
            _postComment = postComment;
            _leaveMessage = leaveMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<WebToDayStatisticsDto> GetWebToDayStatisticsAsync()
        {
            if (_monitorContext.Value.HourStatistics.LastTime >= DateTime.Now)
                return _monitorContext.Value.HourStatistics.ToMap<WebToDayStatisticsDto>();

            var yesterday = DateTime.Now.Date.AddDays(-1);
            var today = DateTime.Now.Date;

            var yesterdayStatistics = await _webDayStatistics.GetAsync(yesterday) ?? new WebDayStatistics();

            // 浏览量计算
            {
                _monitorContext.Value.HourStatistics.PVBrowse = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync();
                // 与昨日计算百分比
                _monitorContext.Value.HourStatistics.PVBrowsePercent = CalcHourStatisticsPercent(yesterdayStatistics.PVBrowse, _monitorContext.Value.HourStatistics.PVBrowse);
            }

            // 访客计算
            {
                var startTime = DateTime.Now.AddMinutes(-5);
                _monitorContext.Value.HourStatistics.OnlineUsers = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= startTime).GroupBy(x => x.ClientOrign).CountAsync();
                _monitorContext.Value.HourStatistics.UVBrowse = await _browseLog.Value.GetAll().Where(x => x.CreateTime >= today).GroupBy(x => x.ClientOrign).CountAsync();
                // 与昨日计算百分比
                _monitorContext.Value.HourStatistics.UVBrowsePercent = CalcHourStatisticsPercent(yesterdayStatistics.UVBrowse, _monitorContext.Value.HourStatistics.UVBrowse);
            }

            // 评论、留言计算
            {
                _monitorContext.Value.HourStatistics.CommentMessage = await _postComment.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync();
                _monitorContext.Value.HourStatistics.CommentMessage += await _leaveMessage.Value.GetAll().Where(x => x.CreateTime >= today).CountAsync();
                // 与昨日计算百分比
                _monitorContext.Value.HourStatistics.CommentMessagePercent = CalcHourStatisticsPercent(yesterdayStatistics.CommentMessage, _monitorContext.Value.HourStatistics.CommentMessage);
            }

            // 平均响应耗时
            {
                var requestStartTime = DateTime.Now.Date;
                var requestEndTime = DateTime.Now;
                var requestCount = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= requestStartTime && x.CreateTime < requestEndTime).CountAsync();
                if (requestCount > 0)
                {
                    var totalElapsedMilliseconds = await _requestLog.Value.GetAll().Where(x => x.CreateTime >= requestStartTime && x.CreateTime < requestEndTime).SumAsync(x => x.ElapsedMilliseconds);

                    var oldElapsedMilliseconds = _monitorContext.Value.HourStatistics.ElapsedMilliseconds;
                    _monitorContext.Value.HourStatistics.ElapsedMilliseconds = (int)Math.Ceiling((double)totalElapsedMilliseconds / requestCount);
                    _monitorContext.Value.HourStatistics.ElapsedMillisecondsDifference = oldElapsedMilliseconds > 0 ? oldElapsedMilliseconds - _monitorContext.Value.HourStatistics.ElapsedMilliseconds : 0;
                }
            }

            _monitorContext.Value.HourStatistics.LastTime = DateTime.Now.AddSeconds(10);

            return _monitorContext.Value.HourStatistics.ToMap<WebToDayStatisticsDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<WorldBrowseMapDataDto>> GetWorldBrowseMapListAsync()
        {
            var qyert = _browseWorldMap.GetAll()
                                       .Where(x => x.Count > 0)
                                       .Select(x => new WorldBrowseMapDataDto()
                                       {
                                           Country = x.Country,
                                           Count = x.Count
                                       });

            var list = await qyert.ToListAsync();

            return list.OrderByDescending(x => x.Count).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public async Task<PageResultDto<BrowseStatisticsDataDto>> GetBrowseStatisticsListAsync(int pageIndex, int pageSize, bool sum)
        {
            var filter = _browseStatistics.GetAll().Where(x => x.Count > 0);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<BrowseStatisticsDataDto>(count);

            var query = filter.OrderByDescending(x => x.Count)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new BrowseStatisticsDataDto()
                              {
                                  Route = x.Route,
                                  PageName = x.PageName,
                                  Count = x.Count
                              });

            var list = await query.ToListAsync();

            BrowseStatisticsDataDto? summary = null;
            if (sum)
            {
                summary = new BrowseStatisticsDataDto
                {
                    Count = await filter.SumAsync(x => x.Count)
                };
            }

            return new PageResultDto<BrowseStatisticsDataDto>(count, summary, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public async Task<PageResultDto<RefererStatisticsDataDto>> GetRefererStatisticsListAsync(int pageIndex, int pageSize, bool sum)
        {
            var filter = _refererStatistics.GetAll().Where(x => x.Count > 0);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<RefererStatisticsDataDto>(count);

            var query = filter.OrderByDescending(x => x.Count)
                              .PageBy(pageIndex, pageSize)
                              .Select(x => new RefererStatisticsDataDto()
                              {
                                  Referer = x.Referer,
                                  Domain = x.Domain,
                                  Count = x.Count
                              });

            var list = await query.ToListAsync();

            RefererStatisticsDataDto? summary = null;
            if (sum)
            {
                summary = new RefererStatisticsDataDto
                {
                    Count = await filter.SumAsync(x => x.Count)
                };
            }

            return new PageResultDto<RefererStatisticsDataDto>(count, summary, list);
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
            else if (old == @new)
                return 0d;

            var val = @new - old;
            var percent = Math.Ceiling((Math.Abs(val) / (double)old) * 10000d) / 100;
            return val > 0 ? percent : -percent;
        }
    }
}
