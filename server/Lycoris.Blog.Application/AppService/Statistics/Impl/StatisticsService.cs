using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Blog.Application.AppService.Statistics.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class StatisticsService : IStatisticsService
    {
        private readonly IRepository<User, long> _user;
        private readonly IRepository<Post, long> _post;
        private readonly IRepository<RequestLog, long> _requestLog;
        private readonly IRepository<BrowseLog, long> _browseLog;
        private readonly IRepository<LeaveMessage, int> _leaveMessage;

        public StatisticsService(IRepository<User, long> user,
                                 IRepository<Post, long> post,
                                 IRepository<RequestLog, long> requestLog,
                                 IRepository<BrowseLog, long> browseLog,
                                 IRepository<LeaveMessage, int> leaveMessage)
        {
            _user = user;
            _post = post;
            _requestLog = requestLog;
            _browseLog = browseLog;
            _leaveMessage = leaveMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUserCountAsync() => await _user.GetAll().Where(x => x.IsAdmin == false).CountAsync();
    }
}
