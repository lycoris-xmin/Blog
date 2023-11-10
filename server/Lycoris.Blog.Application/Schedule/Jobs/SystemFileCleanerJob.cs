using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Github;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Core.Minio;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions;
using Microsoft.EntityFrameworkCore;
using Octokit;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("系统文件清理", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0 1 * * ?")]
    public class SystemFileCleanerJob : BaseJob
    {
        private readonly IConfigurationRepository _configuration;
        private readonly IRepository<StaticFile, long> _staticFile;
        private readonly Lazy<IGithubService> _github;
        private readonly Lazy<IMinioService> _minio;

        public SystemFileCleanerJob(ILycorisLoggerFactory factory,
                                    IConfigurationRepository configuration,
                                    IRepository<StaticFile, long> staticFile,
                                    Lazy<IGithubService> github,
                                    Lazy<IMinioService> minio) : base(factory.CreateLogger<SystemFileCleanerJob>())
        {
            _configuration = configuration;
            _staticFile = staticFile;
            _github = github;
            _minio = minio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task HandlerWorkAsync()
        {
            var config = await GetConfigurationAsync();

            // 静态文件清理
            await StaticFileHandlerAsync(config);

            // 缓存文件清理
            TempFileHandler();

            // 日志文件清理
            LogFileHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<SystemFileClearConfiguration> GetConfigurationAsync()
        {
            var config = await _configuration.GetConfigurationAsync<SystemSettingsConfiguration>(AppConfig.SystemSettings);
            return config!.SystemFileClear;
        }

        #region 静态文件清理

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task StaticFileHandlerAsync(SystemFileClearConfiguration config)
        {
            var checkTime = DateTime.Now.AddDays(-config.StaticFile).Date;
            var filter = _staticFile.GetAll().Where(x => x.Use == false).Where(x => x.LastUpdateTime.HasValue && x.LastUpdateTime.Value <= checkTime);

            var pageIndex = 1;
            var pageSize = 50;

            do
            {
                var list = await filter.PageBy(pageIndex, pageSize).ToListAsync() ?? new List<StaticFile>();

                var deleteFiles = await RemoveGitHubFilesAsync(list);

                deleteFiles = await RemoveMinioFilesAsync(deleteFiles);

                deleteFiles = RemoveLocalFiles(deleteFiles);

                if (deleteFiles.HasValue())
                {
                    // 删除数据
                    var sql = $"DELETE FROM {_staticFile.TableName} WHERE Id IN ({string.Join(",", deleteFiles!.Select(x => x.Id))})";
                    await _staticFile.ExecuteNonQueryAsync(sql);
                }

                if (list.Count < pageSize)
                    break;

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<List<StaticFile>?> RemoveGitHubFilesAsync(List<StaticFile>? list)
        {
            if (!list.HasValue())
                return null;

            var failedIds = new List<long>();
            var _list = list!.Where(x => x.UploadChannel == FileUploadChannelEnum.Github).ToList();

            foreach (var item in _list!)
            {
                try
                {
                    await _github.Value.RemoveFileAsync(item.FileSha, item.PathUrl);
                }
                catch (NotFoundException)
                {
                    this.JobLogger.Warn($"github delete file failed: not found file by {item.PathUrl}");
                }
                catch (Exception ex)
                {
                    this.JobLogger.Error($"github delete file({item.PathUrl}) failed", ex);
                    failedIds.Add(item.Id);
                }

                Thread.CurrentThread.Join(1000);
            }

            return list!.Where(x => !failedIds.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<List<StaticFile>?> RemoveMinioFilesAsync(List<StaticFile>? list)
        {
            if (!list.HasValue())
                return null;

            var failedIds = new List<long>();
            var _list = list!.Where(x => x.UploadChannel == FileUploadChannelEnum.Minio).ToList();

            foreach (var item in _list!)
            {
                try
                {
                    await _minio.Value.RemoveFileAsync(x => x.WithFileUrl(item.PathUrl));
                }
                catch (Exception ex)
                {
                    this.JobLogger.Error($"minio delete file({item.PathUrl}) failed", ex);
                    failedIds.Add(item.Id);
                }

                Thread.CurrentThread.Join(1000);
            }

            return list!.Where(x => !failedIds.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<StaticFile>? RemoveLocalFiles(List<StaticFile>? list)
        {
            if (!list.HasValue())
                return null;

            var failedIds = new List<long>();

            foreach (var item in list!)
            {
                var filePath = Path.Combine(AppSettings.Path.WebRootPath, item.Path.TrimStart('/'), item.FileName);

                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    this.JobLogger.Error($"local delete file({filePath}) failed", ex);
                    failedIds.Add(item.Id);
                }
            }

            return list.Where(x => !failedIds.Contains(x.Id)).ToList();
        }

        #endregion

        #region 缓存文件清理

        /// <summary>
        /// 
        /// </summary>
        private static void TempFileHandler()
        {
            var files = Directory.GetFiles(AppSettings.Path.Temp);

            if (files == null || files.Length == 0)
                return;

            foreach (var item in files)
            {
                var time = File.GetCreationTime(item);
                if (time.AddDays(1) < DateTime.Now.Date)
                    File.Delete(item);
            }
        }

        #endregion

        #region 日志文件清理

        /// <summary>
        /// 
        /// </summary>
        private static void LogFileHandler()
        {
            var path = Path.Combine(AppSettings.Path.AppData, "logs");
            var files = Directory.GetFiles(path);


            if (files == null || files.Length == 0)
                return;

            foreach (var item in files)
            {
                var time = File.GetCreationTime(item);
                if (time.AddDays(7) < DateTime.Now.Date)
                    File.Delete(item);
            }
        }

        #endregion
    }
}
