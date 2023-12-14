using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Quartz.Extensions;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("数据库垃圾数据定时清理", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0 1 * * ?")]
    public class DBCleanerJob : BaseJob
    {
        private readonly IConfigurationRepository _configuration;
        private readonly IRepository<RequestLog, long> _requestLog;
        private readonly IRepository<BrowseLog, long> _browseLog;
        private readonly IRepository<PostComment, long> _postComment;
        private readonly IRepository<LeaveMessage, int> _leaveMessage;

        public DBCleanerJob(ILycorisLoggerFactory factory,
                            IConfigurationRepository configuration,
                            IRepository<RequestLog, long> requestLog,
                            IRepository<BrowseLog, long> browseLog,
                            IRepository<PostComment, long> postComment,
                            IRepository<LeaveMessage, int> leaveMessage) : base(factory.CreateLogger<DBCleanerJob>())
        {
            _configuration = configuration;
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
            var config = await GetConfigurationAsync();

            await RequestLogCleaningAsync(config.RequestLog);

            await BrowseLogCleaningAsync(config.BrowseLog);

            await PostCommentCleaningAsync(config.PostComment);

            await LeaveMessageCleaningAsync(config.LeaveMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<DataClearConfiguration> GetConfigurationAsync()
        {
            var config = await _configuration.GetConfigurationAsync<OtherSettingsConfiguration>(AppConfig.OtherSetting);
            return config!.DataClear;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDay"></param>
        /// <returns></returns>
        private async Task RequestLogCleaningAsync(int checkDay)
        {
            if (checkDay == 0)
                return;

            try
            {
                var sql = $"DELETE FROM {_requestLog.TableName} WHERE {nameof(RequestLog.CreateTime)} < '{DateTime.Now.AddDays(-checkDay):yyyy-MM-dd 00:00:00}' ORDER BY {nameof(RequestLog.CreateTime)} LIMIT 100;";

                var count = await _requestLog.ExecuteNonQueryAsync(sql);

                while (count == 100)
                    count = await _requestLog.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                this.JobLogger.Error("", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDay"></param>
        /// <returns></returns>
        private async Task BrowseLogCleaningAsync(int checkDay)
        {
            if (checkDay == 0)
                return;

            try
            {
                var sql = $"DELETE FROM {_browseLog.TableName} WHERE {nameof(BrowseLog.CreateTime)} < '{DateTime.Now.AddDays(-checkDay):yyyy-MM-dd 00:00:00}' ORDER BY {nameof(BrowseLog.CreateTime)} LIMIT 100;";
                var count = await _browseLog.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _browseLog.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                this.JobLogger.Error("", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDay"></param>
        /// <returns></returns>
        private async Task PostCommentCleaningAsync(int checkDay)
        {
            if (checkDay == 0)
                return;

            try
            {
                var sql = $"DELETE FROM {_postComment.TableName} WHERE {nameof(PostComment.CreateTime)} < '{DateTime.Now.AddDays(-checkDay):yyyy-MM-dd 00:00:00}' ORDER BY {nameof(PostComment.CreateTime)} LIMIT 100;";
                var count = await _postComment.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _postComment.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                this.JobLogger.Error("", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDay"></param>
        /// <returns></returns>
        private async Task LeaveMessageCleaningAsync(int checkDay)
        {
            if (checkDay == 0)
                return;

            try
            {
                var sql = $"DELETE FROM {_leaveMessage.TableName} WHERE {nameof(LeaveMessage.Status)} <> 0 OR {nameof(LeaveMessage.CreateTime)} < '{DateTime.Now.AddDays(-checkDay):yyyy-MM-dd 00:00:00}' ORDER BY {nameof(LeaveMessage.CreateTime)} LIMIT 100;";
                var count = await _leaveMessage.ExecuteNonQueryAsync(sql);
                while (count == 100)
                    count = await _leaveMessage.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                this.JobLogger.Error("", ex);
            }
        }
    }
}
