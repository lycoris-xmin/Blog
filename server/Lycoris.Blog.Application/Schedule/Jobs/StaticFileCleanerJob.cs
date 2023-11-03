using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Github;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Core.Minio;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Lycoris.Quartz.Extensions.Job;
using Microsoft.EntityFrameworkCore;
using Octokit;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    [QuartzJob("静态文件清理", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0 1 * * ?")]
    public class StaticFileCleanerJob : BaseJob
    {
        private readonly IRepository<StaticFile, long> _staticFile;
        private readonly Lazy<IGithubService> _github;
        private readonly Lazy<IMinioService> _minio;

        public StaticFileCleanerJob(ILycorisLoggerFactory factory,
                                    IRepository<StaticFile, long> staticFile,
                                    Lazy<IGithubService> github,
                                    Lazy<IMinioService> minio) : base(factory.CreateLogger<StaticFileCleanerJob>())
        {
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
            var filter = _staticFile.GetAll().Where(x => x.Use == false);

            var pageIndex = 1;
            var pageSize = 50;

            do
            {
                var list = await filter.PageBy(pageIndex, pageSize).ToListAsync() ?? new List<StaticFile>();

                var deleteFiles = await RemoveGitHubFilesAsync(list);

                deleteFiles = await RemoveMinioFilesAsync(deleteFiles);

                deleteFiles = RemoveLocalFiles(deleteFiles);

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
            var _list = list!.Where(x => x.SaveChannel == FileSaveChannelEnum.Github).ToList();

            foreach (var item in _list!)
            {
                try
                {
                    await _github.Value.RemoveFileAsync(item.FileSha, item.PathUrl);
                }
                catch (NotFoundException)
                {
                    _logger.Warn($"github delete file failed: not found file by {item.PathUrl}");
                    failedIds.Add(item.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"github delete file({item.PathUrl}) failed", ex);
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
            var _list = list!.Where(x => x.SaveChannel == FileSaveChannelEnum.Minio).ToList();

            foreach (var item in _list!)
            {
                try
                {
                    await _minio.Value.RemoveFileAsync(x => x.WithFileUrl(item.PathUrl));
                }
                catch (Exception ex)
                {
                    _logger.Error($"minio delete file({item.PathUrl}) failed", ex);
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
                    _logger.Error($"local delete file({filePath}) failed", ex);
                    failedIds.Add(item.Id);
                }
            }

            return list.Where(x => !failedIds.Contains(x.Id)).ToList();
        }
    }
}
