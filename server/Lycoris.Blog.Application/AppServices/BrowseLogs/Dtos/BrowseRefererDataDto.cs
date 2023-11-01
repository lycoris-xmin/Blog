namespace Lycoris.Blog.Application.AppServices.BrowseLogs.Dtos
{
    public class BrowseRefererDataDto
    {
        public int Id { get; set; }

        public string Domain { get; set; } = "";

        public string Referer { get; set; } = "";

        public int Count { get; set; }
    }
}
