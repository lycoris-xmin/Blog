using Lycoris.Blog.Application.Schedule.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Application.SignalR.Hubs;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Core.SharpFastZip;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    [QuartzJob("检测文件归档", Standby = true)]
    public class BackupFileJob : BaseJob
    {
        private readonly ISharpFastZipService _sharpFastZip;
        private readonly IHubContext<DashboardHub> _hubContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="sharpFastZip"></param>
        public BackupFileJob(ILycorisLoggerFactory factory,
                             ISharpFastZipService sharpFastZip,
                             IHubContext<DashboardHub> hubContext) : base(factory.CreateLogger<BackupFileJob>())
        {
            _sharpFastZip = sharpFastZip;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            var args = this.Context.GetJobArgs<BackupFileJobModel>();
            if (args == null)
            {
                this.JobLogger.Error("job arguments is null");
                return;
            }

            if (args.SourceFilePath.IsNullOrEmpty() || args.ZipFilePath.IsNullOrEmpty())
            {
                // 如果路径为空不处理
                this.JobLogger.Error($"path must not be null:{args.ToJson()}");
                return;
            }

            // 生成压缩文件
            _sharpFastZip.CreateZipFile(args.ZipFilePath!, args.SourceFilePath!);

            // 通知前端
            await _hubContext.Clients.All.SendAsync("downloadAll", Path.GetFileName(args.ZipFilePath!));
        }
    }
}
