using Microsoft.AspNetCore.SignalR;

namespace Lycoris.Blog.Application.SignalR.Home
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        public async Task SendMessage(string user, string message)
        {
            //
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
