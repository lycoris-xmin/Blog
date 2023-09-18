namespace Lycoris.Blog.Application.AppService.BrowseLogs.Dtos
{
    public class BrowseRefererDataDto
    {
        public int Id { get; set; }

        public string Domain { get; set; } = "";

        public string Referer { get; set; } = "";

        public int Count { get; set; }
    }
}
