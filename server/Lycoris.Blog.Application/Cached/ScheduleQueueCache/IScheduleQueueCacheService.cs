using Lycoris.Blog.Application.Cached.ScheduleQueueCache.Dtos;

namespace Lycoris.Blog.Application.Cached.ScheduleQueueCache
{
    public interface IScheduleQueueCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        Task EnqueueAsync(Action<ScheduleQueueDto> configure);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Task EnqueueAsync(ScheduleTypeEnum type, string value, DateTime? time = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        Task EnqueueAsync<T>(ScheduleTypeEnum type, T value, DateTime? time = null) where T : class;

        /// <summary>
        /// 
        /// </summary>
        Task EnqueueAsync(ScheduleQueueDto value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ScheduleQueueDto?> DequeueAsync();
    }
}
