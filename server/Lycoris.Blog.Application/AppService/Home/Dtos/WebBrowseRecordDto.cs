namespace Lycoris.Blog.Application.AppService.Home.Dtos
{
    public class WebBrowseRecordDto
    {
        public string ClientOrign { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string PageName { get; set; } = string.Empty;

        public string Referer { get; set; } = string.Empty;
    }
}
