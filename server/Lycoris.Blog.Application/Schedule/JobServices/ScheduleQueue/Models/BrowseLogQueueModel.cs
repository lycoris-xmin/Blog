namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class BrowseLogQueueModel
    {
        public BrowseLogQueueModel() { }

        public BrowseLogQueueModel(string Path, string PageName, string UserAgent, string? Referer, string? Ip)
        {
            this.Path = Path;
            this.PageName = PageName;
            this.UserAgent = UserAgent;
            this.Referer = Referer ?? "";
            this.Ip = Ip ?? "";
        }

        public string Path { get; set; } = "";

        public string PageName { get; set; } = "";

        public string UserAgent { get; set; } = "";

        public string? Referer { get; set; }

        public string? Ip { get; set; }
    }
}
