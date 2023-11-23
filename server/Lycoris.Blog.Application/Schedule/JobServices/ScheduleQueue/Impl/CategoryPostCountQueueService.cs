using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Application.Schedule.Shared;
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
        public JobLogger? JobLogger { get; set; }


        private readonly IRepository<Category, int> _category;
        private readonly IRepository<Post, long> _post;

        public CategoryPostCountQueueService(IRepository<Category, int> category, IRepository<Post, long> post)
        {
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
            {
                this.JobLogger!.Error($"can not find category id by data:{data}");
                return;
            }

            var category = await _category.GetAsync(categoryId!.Value);
            if (category == null)
            {
                this.JobLogger!.Error($"can not find category by id:{categoryId}");
                return;
            }

            var count = await _post.GetAll().Where(x => x.Category == categoryId!.Value).Where(x => x.IsPublish).CountAsync();
            if (category.PostCount == count)
            {
                this.JobLogger!.Warn($"the number({count}) of articles in the category({category.Name}) is equal to the current number and does not need to be updated.");
                return;
            }

            category.PostCount = count;
            await _category.UpdateFieIdsAsync(category, x => x.PostCount);
        }
    }
}
