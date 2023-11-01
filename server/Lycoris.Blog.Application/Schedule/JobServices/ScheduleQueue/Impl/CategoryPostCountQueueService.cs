using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, MultipleNamed = "CategoryPostCount")]
    public class CategoryPostCountQueueService : IScheduleQueueService
    {
        public IJobExecutionContext? JobContext { get; set; }

        private readonly ILycorisLogger _logger;
        private readonly IRepository<Category, int> _category;
        private readonly IRepository<Post, long> _post;

        public CategoryPostCountQueueService(ILycorisLoggerFactory factory,
                                           IRepository<Category, int> category,
                                           IRepository<Post, long> post)
        {
            _logger = factory.CreateLogger<LeaveMessageQueueService>();
            _category = category;
            _post = post;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task JobDoWorkAsync(string? data, DateTime? time)
        {
            var categoryId = data.ToTryInt();
            if (!categoryId.HasValue || categoryId.Value <= 0)
                return;

            var category = await _category.GetAsync(categoryId!.Value);
            if (category == null)
                return;

            var count = await _post.GetAll().Where(x => x.Category == categoryId!.Value).Where(x => x.IsPublish).CountAsync();
            if (category.PostCount == count)
                return;

            category.PostCount = count;
            await _category.UpdateFieIdsAsync(category, x => x.PostCount);
        }
    }
}
