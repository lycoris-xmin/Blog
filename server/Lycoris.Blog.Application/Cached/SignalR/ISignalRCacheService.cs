using Lycoris.Blog.Application.SignalR.Shared.Models;

namespace Lycoris.Blog.Application.Cached.SignalR
{
    public interface ISignalRCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        SignalRConnectionModel? GetSignalRConnection(string connectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        void SetSignalRConnection(string connectionId, SignalRConnectionModel value, TimeSpan time);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        void RemoveSignalRConnection(string connectionId);
    }
}
