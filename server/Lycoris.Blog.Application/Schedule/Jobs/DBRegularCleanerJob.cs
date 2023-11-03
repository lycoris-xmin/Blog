using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Quartz.Extensions.Job;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("数据库垃圾数据定时清理", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0 1 * * ?")]
    public class DBRegularCleanerJob : BaseJob
    {
        private readonly IRepository<RequestLog, long> _requestLog;
        private readonly IRepository<BrowseLog, long> _browseLog;
        private readonly IRepository<PostComment, long> _postComment;
        private readonly IRepository<LeaveMessage, int> _leaveMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestLog"></param>
        /// <param name="browseLog"></param>
        public DBRegularCleanerJob(ILycorisLoggerFactory factory,
                                    IRepository<RequestLog, long> requestLog,
                                    IRepository<BrowseLog, long> browseLog,
                                    IRepository<PostComment, long> postComment,
                                    IRepository<LeaveMessage, int> leaveMessage) : base(factory.CreateLogger<DBRegularCleanerJob>())
        {
            _requestLog = requestLog;
            _browseLog = browseLog;
            _postComment = postComment;
            _leaveMessage = leaveMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            var checkTime = DateTime.Now.AddMonths(-6).Date;

            await RequestLogRegularCleaningAsync(checkTime);

            await BrowseLogRegularCleaningAsync(checkTime);

            await PostCommentRegularCleaningAsync(checkTime);

            await LeaveMessageRegularCleaningAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        private async Task RequestLogRegularCleaningAsync(DateTime checkTime)
        {
            try
            {
                var errorCheckTime = checkTime.AddMonths(-2);

                var sql = $"DELETE FROM {_requestLog.TableName} WHERE (Success = 1 AND CreateTime < '{checkTime:yyyy-MM-dd HH:mm:ss}') OR (Success = 0 AND CreateTime < '{errorCheckTime:yyyy-MM-dd HH:mm:ss}') ORDER BY CreateTime LIMIT 100;";

                var count = await _requestLog.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _requestLog.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        private async Task BrowseLogRegularCleaningAsync(DateTime checkTime)
        {
            try
            {
                var sql = $"DELETE FROM {_browseLog.TableName} WHERE CreateTime < '{checkTime:yyyy-MM-dd HH:mm:ss}' ORDER BY CreateTime LIMIT 100;";
                var count = await _browseLog.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _browseLog.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        private async Task PostCommentRegularCleaningAsync(DateTime checkTime)
        {
            try
            {
                var sql = $"DELETE FROM {_postComment.TableName} WHERE OriginalContent <> '' AND CreateTime < '{checkTime:yyyy-MM-dd HH:mm:ss}' ORDER BY CreateTime LIMIT 100;";
                var count = await _postComment.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _postComment.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        private async Task LeaveMessageRegularCleaningAsync()
        {
            try
            {
                var sql = $"DELETE FROM {_leaveMessage.TableName} WHERE Status <> 0 AND CreateTime < '{DateTime.Now:yyyy-MM-dd 00:00:00}' ORDER BY CreateTime LIMIT 100;";
                var count = await _leaveMessage.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _leaveMessage.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
            }
        }
    }
}
