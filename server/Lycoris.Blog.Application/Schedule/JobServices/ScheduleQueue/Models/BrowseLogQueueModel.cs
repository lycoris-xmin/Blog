using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Contexts;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class BrowseLogQueueModel
    {
        public BrowseLogQueueModel() { }

        public BrowseLogQueueModel(BrowseLog data, RequestContext request, bool? IsPost)
        {
            this.Route = data.Route;
            this.PageName = data.PageName;
            this.UserAgent = data.UserAgent;
            this.Referer = data.Referer ?? "";
            this.Ip = request.RequestIP ?? "";
            this.UserId = request.User?.Id;
            this.IsPost = IsPost ?? false;
        }

        public string Route { get; set; } = "";

        public string PageName { get; set; } = "";

        public string UserAgent { get; set; } = "";

        public string? Referer { get; set; }

        public string? Ip { get; set; }

        public long? UserId { get; set; }

        public bool IsPost { get; set; }
    }
}
