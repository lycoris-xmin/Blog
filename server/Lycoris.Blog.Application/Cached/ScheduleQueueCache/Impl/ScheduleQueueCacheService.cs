using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.Cached.ScheduleQueueCache.Dtos;
using Lycoris.Blog.Common;
using Lycoris.CSRedisCore.Extensions;
using System.Collections.Concurrent;

namespace Lycoris.Blog.Application.Cached.ScheduleQueueCache.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ScheduleQueueCacheService : IScheduleQueueCacheService
    {
        private readonly ConcurrentQueue<ScheduleQueueDto> _queue = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public Task EnqueueAsync(Action<ScheduleQueueDto> configure)
        {
            var value = new ScheduleQueueDto();
            configure.Invoke(value);
            return EnqueueAsync(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Task EnqueueAsync(ScheduleTypeEnum type, string value, DateTime? time = null)
        {
            return EnqueueAsync(new ScheduleQueueDto()
            {
                Type = type,
                Data = value,
                Time = time ?? DateTime.Now
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Task EnqueueAsync<T>(ScheduleTypeEnum type, T value, DateTime? time = null) where T : class
        {
            return EnqueueAsync(new ScheduleQueueDto()
            {
                Type = type,
                Data = value.ToJson(),
                Time = time ?? DateTime.Now
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task EnqueueAsync(ScheduleQueueDto value)
        {
            if (AppSettings.Redis.Use)
                await RedisCache.Utils.EnqueueAsync("ScheduleQueue", value);
            else
                _queue.Enqueue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ScheduleQueueDto?> DequeueAsync()
        {
            if (AppSettings.Redis.Use)
                return await RedisCache.Utils.DequeueAsync<ScheduleQueueDto>("ScheduleQueue");
            else
                return _queue.TryDequeue(out ScheduleQueueDto? result) ? result ?? null : null;
        }
    }
}

