using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "PostStatistics")]
    public class PostStatisticsQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        private readonly IRepository<Post, long> _post;
        private readonly IRepository<PostStatistics, long> _postStatistics;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <param name="postStatistics"></param>
        public PostStatisticsQueueService(IRepository<Post, long> post, IRepository<PostStatistics, long> postStatistics)
        {
            _post = post;
            _postStatistics = postStatistics;
        }

        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var dto = data?.ToObject<PostStaticQueueDto>();
            if (dto == null)
                return;

            var post = await _post.GetAsync(dto.PostId);
            if (post == null)
                return;

            var statistics = await _postStatistics.GetAsync(dto.PostId) ?? new PostStatistics() { Browse = 0, Comment = 0 };

            statistics.Comment += dto.StaticType == PostStaticTypeEnum.Comment ? 1 : 0;
            statistics.Browse += dto.StaticType == PostStaticTypeEnum.Browse ? 1 : 0;

            if (statistics.Id > 0)
            {
                await _postStatistics.UpdateAsync(statistics);
            }
            else
            {
                statistics.Id = dto.PostId;
                await _postStatistics.CreateAsync(statistics);
            }
        }
    }
}
