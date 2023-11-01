using Lycoris.Blog.Application.SignalR.Shared.Models;
using Lycoris.Blog.EntityFrameworkCore.Tables;

namespace Lycoris.Blog.Application.SignalR.Shared
{
    public interface ISignalRService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<User?> AnalyzeTokenAsync(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task<SignalRConnectionModel?> GetSignalRConnectionAsync(string connectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<SignalRConnectionModel?> GetSignalRConnectionAsync(long userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="exceptConnection"></param>
        /// <returns></returns>
        Task<string[]> GetUserAllSignalRConnectionAsync(long userId, params string[] exceptConnection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<SignalRConnectionModel> AddOrUpdateSignalRConnectionAsync(SignalRConnectionModel data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<string>> CleanUserDisconnectedAsync(string connectionId, long userId);
    }
}
