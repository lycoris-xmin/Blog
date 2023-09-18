using Lycoris.Blog.Application.SignalR.Shared.Dtos;

namespace Lycoris.Blog.Application.Cached.SignalRCache
{
    public interface ISignalRCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task<SignalRConnectionDto?> GetSignalRConnectionAsync(string connectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Task SetSignalRConnectionAsync(string connectionId, SignalRConnectionDto value, TimeSpan time);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task RemoveSignalRConnectionAsync(string connectionId);
    }
}
