namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos
{
    public class BrowseLogQueueDto
    {
        public BrowseLogQueueDto() { }

        public BrowseLogQueueDto(string Path, string? Referer)
        {
            this.Path = Path;
            this.Referer = Referer ?? "";
        }

        public string Path { get; set; } = "";

        public string? Referer { get; set; }
    }
}
