namespace Lycoris.Blog.Application.AppService.LoginFailedRecords.Dtos
{
    public class LoginFailedRecordDto
    {
        public LoginFailedRecordDto() { }


        public LoginFailedRecordDto(int Count, DateTime? FreezeTime)
        {
            this.Count = Count;
            this.FreezeTime = FreezeTime;
        }

        public int Count { get; set; }

        public DateTime? FreezeTime { get; set; }
    }
}
