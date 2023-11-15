namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class BrowseLogQueueModel
    {
        public BrowseLogQueueModel() { }

        public BrowseLogQueueModel(string Path, string? Referer)
        {
            this.Path = Path;
            this.Referer = Referer ?? "";
        }

        public string Path { get; set; } = "";

        public string? Referer { get; set; }
    }
}
