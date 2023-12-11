namespace Lycoris.Blog.Application.AppServices.Home.Dtos
{
    public class WebBrowseRecordDto
    {
        public string ClientOrign { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public string PageName { get; set; } = string.Empty;

        public string Referer { get; set; } = string.Empty;

        public bool? IsPost { get; set; }
    }
}
