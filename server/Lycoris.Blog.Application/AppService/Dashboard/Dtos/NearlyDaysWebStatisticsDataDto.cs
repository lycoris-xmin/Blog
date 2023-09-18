namespace Lycoris.Blog.Application.AppService.Dashboard.Dtos
{
    public class NearlyDaysWebStatisticsDataDto
    {
        public NearlyDaysWebStatisticsDataDto() { }

        public NearlyDaysWebStatisticsDataDto(DateTime Day)
        {
            this.Day = Day;
        }

        public DateTime Day { get; set; }

        public int PVBrowse { get; set; } = 0;

        public int UVBrowse { get; set; } = 0;

        public int Api { get; set; } = 0;

        public int ErrorApi { get; set; } = 0;
    }
}
