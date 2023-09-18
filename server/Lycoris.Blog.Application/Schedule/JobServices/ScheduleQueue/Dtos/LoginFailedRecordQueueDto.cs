using Lycoris.Blog.Application.AppService.LoginFailedRecords.Dtos;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos
{
    public class LoginFailedRecordQueueDto
    {
        public LoginFailedRecordQueueDto() { }

        public LoginFailedRecordQueueDto(string Email, LoginFailedRecordDto data)
        {
            this.Email = Email;
            Count = data.Count;
            FreezeTime = data.FreezeTime;
        }

        public string Email { get; set; } = "";

        public int Count { get; set; }

        public DateTime? FreezeTime { get; set; }
    }
}
