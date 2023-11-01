using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Cache.ScheduleQueue.Models;
using Lycoris.Common.Extensions;
using System.Collections.Concurrent;

namespace Lycoris.Blog.Cache.ScheduleQueue.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ScheduleQueueCacheService : IScheduleQueueCacheService
    {
        private readonly ConcurrentQueue<ScheduleQueueCacheModel> _queue = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public void Enqueue(Action<ScheduleQueueCacheModel> configure)
        {
            var value = new ScheduleQueueCacheModel();
            configure.Invoke(value);
            Enqueue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void Enqueue(ScheduleTypeEnum type, string value, DateTime? time = null) => Enqueue(new ScheduleQueueCacheModel() { Type = type, Data = value, Time = time ?? DateTime.Now });

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void Enqueue<T>(ScheduleTypeEnum type, T value, DateTime? time = null) where T : class => Enqueue(new ScheduleQueueCacheModel() { Type = type, Data = value.ToJson(), Time = time ?? DateTime.Now });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Enqueue(ScheduleQueueCacheModel value) => _queue.Enqueue(value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ScheduleQueueCacheModel? Dequeue() => _queue.TryDequeue(out ScheduleQueueCacheModel? result) ? result ?? null : null;
    }
}

