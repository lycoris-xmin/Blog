﻿using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Quartz.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    [DisallowConcurrentExecution]
    [QuartzJob("网站数据统计", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0/5 3 * * ?")]
    public class WebDayStatisticsJob : BaseJob
    {
        private readonly IRepository<WebDayStatistics, DateTime> _webDayStatistics;
        private readonly IRepository<User, long> _user;
        private readonly IRepository<PostComment, long> _postComment;
        private readonly IRepository<LeaveMessage, int> _message;
        private readonly IRepository<RequestLog, long> _requestLog;
        private readonly IRepository<BrowseLog, long> _browseLog;

        public WebDayStatisticsJob(ILycorisLoggerFactory factory,
                                IRepository<WebDayStatistics, DateTime> webDayStatistics,
                                IRepository<User, long> user,
                                IRepository<PostComment, long> postComment,
                                IRepository<LeaveMessage, int> message,
                                IRepository<RequestLog, long> requestLog,
                                IRepository<BrowseLog, long> browseLog) : base(factory.CreateLogger<WebDayStatisticsJob>())
        {
            _webDayStatistics = webDayStatistics;
            _user = user;
            _postComment = postComment;
            _message = message;
            _requestLog = requestLog;
            _browseLog = browseLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            var lastDay = DateTime.Now.AddDays(-1).Date;

            var lastData = await _webDayStatistics.GetAll().OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (lastData != null)
                lastDay = lastData.Id.AddDays(1);

            // 如果当天任务已执行过，则不再重复统计
            // 预防服务重启时，会重复统计的问题
            if (lastDay == DateTime.Now.Date)
                return;

            var beginDate = lastDay;
            var endDate = beginDate.AddDays(1);
            var absoultEndDate = DateTime.Now.Date;

            do
            {
                var toDayData = await _webDayStatistics.GetAll().Where(x => x.Id == beginDate).SingleOrDefaultAsync() ?? new WebDayStatistics() { Id = beginDate };

                toDayData.Api = await _requestLog.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).CountAsync();
                toDayData.ErrorApi = await _requestLog.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).Where(x => x.Success == false).CountAsync();
                toDayData.PVBrowse = await _browseLog.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).CountAsync();
                toDayData.UVBrowse = await _browseLog.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).GroupBy(x => x.ClientOrign).Select(x => 1).SumAsync(x => x);
                toDayData.User = await _user.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).CountAsync();
                toDayData.CommentMessage = await _message.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).CountAsync();
                toDayData.CommentMessage += await _postComment.GetAll().Where(x => x.CreateTime >= beginDate && x.CreateTime < endDate).CountAsync();

                await _webDayStatistics.CreateOrUpdateAsync(toDayData);

                beginDate = beginDate.AddDays(1);
                endDate = endDate.AddDays(1);

                // 休眠一秒
                if (endDate <= absoultEndDate)
                    Thread.CurrentThread.Join(1000);

            } while (endDate <= absoultEndDate);
        }
    }
}
