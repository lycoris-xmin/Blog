using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Cached.ScheduleQueue;
using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.Schedule.Shared;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "PostStatistics")]
    public class PostStatisticsQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        public JobLogger? JobLogger { get; set; }


        private readonly IRepository<Post, long> _post;
        private readonly Lazy<IScheduleQueueCacheService> _scheduleQueueCache;

        public PostStatisticsQueueService(IRepository<Post, long> post, Lazy<IScheduleQueueCacheService> scheduleQueueCache)
        {
            _post = post;
            _scheduleQueueCache = scheduleQueueCache;
        }

        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var model = data?.ToObject<PostStaticQueueModel>();
            if (model == null)
            {
                this.JobLogger!.Error("can not find any data");
                return;
            }

            var post = await _post.GetAsync(model.PostId);
            if (post == null)
            {
                this.JobLogger!.Error($"can not find post by id:{model.PostId}");
                return;
            }

            post.Statistics ??= new PostStatisticsDao();
            post.Statistics.Comment += model.StaticType == PostStaticTypeEnum.Comment ? 1 : 0;
            post.Statistics.Browse += model.StaticType == PostStaticTypeEnum.Browse ? 1 : 0;

            await _post.UpdateFieIdsAsync(post, x => x.Statistics);

            if (model.StaticType == PostStaticTypeEnum.Comment)
                _scheduleQueueCache.Value.Enqueue(ScheduleTypeEnum.WebStatistics, new WebStatisticsQueueModel() { CommentMessage = 1 });
        }
    }
}
