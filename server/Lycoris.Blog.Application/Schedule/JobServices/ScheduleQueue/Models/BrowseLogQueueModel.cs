namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class BrowseLogQueueModel
    {
        public BrowseLogQueueModel() { }

        public BrowseLogQueueModel(string Path, string? Referer, string? Ip)
        {
            this.Path = Path;
            this.Referer = Referer ?? "";
            this.Ip = Ip ?? "";
        }

        public string Path { get; set; } = "";

        public string? Referer { get; set; }

        public string? Ip { get; set; }
    }
}
