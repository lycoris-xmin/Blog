using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.LoginTokens;
using Lycoris.Blog.Application.Cached.SignalRCache;
using Lycoris.Blog.Application.SignalR.Shared.Dtos;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Lycoris.Blog.Application.SignalR.Shared.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class SignalRService : ISignalRService
    {
        private readonly Lazy<ILoginTokenAppService> _loginToken;
        private readonly IRepository<SignalRConnection, int> _signalRConnection;
        private readonly Lazy<IRepository<User, long>> _user;
        private readonly Lazy<ISignalRCacheService> _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <param name="signalRConnection"></param>
        /// <param name="user"></param>
        /// <param name="cache"></param>
        public SignalRService(Lazy<ILoginTokenAppService> loginToken, IRepository<SignalRConnection, int> signalRConnection, Lazy<IRepository<User, long>> user, Lazy<ISignalRCacheService> cache)
        {
            _loginToken = loginToken;
            _signalRConnection = signalRConnection;
            _user = user;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<User?> AnalyzeTokenAsync(string token)
        {
            var (userId, expiredTime, _) = _loginToken.Value.AnalyzeToken(token);
            if (!userId.HasValue || userId.Value <= 0 || !expiredTime.HasValue || expiredTime.Value <= DateTime.Now)
                return null;

            return await _user.Value.GetAsync(userId!.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task<SignalRConnectionDto?> GetSignalRConnectionAsync(string connectionId)
        {
            var cache = await _cache.Value.GetSignalRConnectionAsync(connectionId);
            if (cache == null)
            {
                var data = await _signalRConnection.GetAsync(x => x.ConnectionId == connectionId);

                if (data != null)
                {
                    cache = data.ToMap<SignalRConnectionDto>();
                    await _cache.Value.SetSignalRConnectionAsync(connectionId, cache, TimeSpan.FromMinutes(10));
                }
            }

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="exceptConnection"></param>
        /// <returns></returns>
        public Task<string[]> GetUserAllSignalRConnectionAsync(long userId, params string[] exceptConnection)
        {
            var filter = _signalRConnection.GetAll().Where(x => x.UserId == userId).WhereIf(exceptConnection.HasValue(), x => !exceptConnection.Contains(x.ConnectionId));
            return filter.Where(x => x.Online == true).Select(x => x.ConnectionId).ToArrayAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SignalRConnectionDto?> GetSignalRConnectionAsync(long userId)
        {
            var data = await _signalRConnection.GetAll().Where(x => x.UserId == userId).Where(x => x.DisconnectedTime.HasValue == false && x.Online == true).OrderByDescending(x => x.ConnectedTime).FirstOrDefaultAsync();
            if (data == null)
                return null;

            var cache = data.ToMap<SignalRConnectionDto>();

            await _cache.Value.SetSignalRConnectionAsync(data.ConnectionId, cache, TimeSpan.FromMinutes(10));

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<SignalRConnectionDto> AddOrUpdateSignalRConnectionAsync(SignalRConnectionDto input)
        {
            if (input.Id > 0)
            {
                var data = await _signalRConnection.GetAsync(input.Id) ?? throw new HttpStatusException(HttpStatusCode.BadRequest, $"错误的连接编号:{input.Id}，找不到连接信息");

                data.UserId = input.UserId;
                data.Online = input.Online ?? false;
                data.DisconnectedTime = input.DisconnectedTime;

                await _signalRConnection.UpdateAsync(data);
            }
            else
                input.Id = await _signalRConnection.CreateAndGetIdAsync(input.ToMap<SignalRConnection>());

            // 加入缓存
            await _cache.Value.SetSignalRConnectionAsync(input.ConnectionId, input, TimeSpan.FromMinutes(10));

            return input;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> CleanUserDisconnectedAsync(string connectionId, long userId)
        {
            var otherConnection = await _signalRConnection.GetAll().Where(x => x.ConnectionId != connectionId && x.UserId == userId).ToListAsync();

            var sql = $"DELETE FROM {_signalRConnection.TableName} WHERE ConnectionId <> '{connectionId}' AND UserId = {userId};";
            await _signalRConnection.ExecuteNonQueryAsync(sql);

            return otherConnection.Where(x => !x.DisconnectedTime.HasValue).Select(x => x.ConnectionId).ToList();
        }
    }
}
