using Lycoris.Blog.Application.SignalR.Shared;
using Lycoris.Blog.Application.SignalR.Shared.Models;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Lycoris.Blog.Application.SignalR.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class DashboardHub : Hub
    {
        public const string ServerMonitorGroup = "ServerMonitor";
        public const string HourStatisticsMonitorGroup = "HourStatisticsMonitor";

        private readonly ISignalRService _signalRService;
        private readonly AppMonitorContext _serverMonitor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalRService"></param>
        /// <param name="serverMonitor"></param>
        public DashboardHub(ISignalRService signalRService, AppMonitorContext serverMonitor)
        {
            _signalRService = signalRService;
            _serverMonitor = serverMonitor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await Clients.Clients(Context.ConnectionId).SendAsync("authroization");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId);
            if (connection != null)
            {
                connection.Online = false;
                connection.DisconnectedTime = DateTime.Now;
                await _signalRService.AddOrUpdateSignalRConnectionAsync(connection);
            }

            // 
            if (_serverMonitor.ServerMonitorConnectionIds.Contains(Context.ConnectionId))
            {
                _serverMonitor.ServerMonitorConnectionIds.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(ServerMonitorGroup, Context.ConnectionId);
            }

            if (_serverMonitor.HourStatisticsConnectionIds.Contains(Context.ConnectionId))
            {
                _serverMonitor.HourStatisticsConnectionIds.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(HourStatisticsMonitorGroup, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HubMethodName("userAuthroization")]
        public async Task UserAuthroizationAsync(string token)
        {
            var user = await _signalRService.AnalyzeTokenAsync(token);
            if (user == null || user.Id == 0)
            {
                await Clients.Caller.SendAsync("refreshToken");
                return;
            }

            var connection = await _signalRService.GetSignalRConnectionAsync(Context.ConnectionId) ?? new SignalRConnectionModel() { ConnectionId = Context.ConnectionId };

            connection.UserId = user.Id;
            connection.NickName = user.NickName;
            connection.Avatar = user.Avatar;
            connection.Online = true;
            connection.ConnectedTime = DateTime.Now;

            await _signalRService.AddOrUpdateSignalRConnectionAsync(connection);

            var otherConnection = await _signalRService.CleanUserDisconnectedAsync(Context.ConnectionId, user.Id);
            if (otherConnection.HasValue())
                await Clients.Clients(otherConnection).SendAsync("logout");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("ConnectServerMonitor")]
        public Task ConnectServerMonitor()
        {
            _serverMonitor.ServerMonitorConnectionIds.Add(Context.ConnectionId);
            return Groups.AddToGroupAsync(Context.ConnectionId, ServerMonitorGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("ConnectHourStatisticsMonitor")]
        public Task ConnectHourStatisticsMonitor()
        {
            _serverMonitor.HourStatisticsConnectionIds.Add(Context.ConnectionId);
            return Groups.AddToGroupAsync(Context.ConnectionId, HourStatisticsMonitorGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HubMethodName("DisConnectHourStatisticsMonitor")]
        public Task DisConnectHourStatisticsMonitor()
        {
            _serverMonitor.HourStatisticsConnectionIds.Remove(Context.ConnectionId);
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, HourStatisticsMonitorGroup);
        }
    }
}
