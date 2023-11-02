using Lycoris.Blog.Application.Cached.ScheduleQueue.Models;

namespace Lycoris.Blog.Application.Cached.ScheduleQueue
{
    public interface IScheduleQueueCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        void Enqueue(Action<ScheduleQueueCacheModel> configure);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        void Enqueue(ScheduleTypeEnum type, string value, DateTime? time = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        void Enqueue<T>(ScheduleTypeEnum type, T value, DateTime? time = null) where T : class;

        /// <summary>
        /// 
        /// </summary>
        void Enqueue(ScheduleQueueCacheModel value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ScheduleQueueCacheModel? Dequeue();
    }
}
