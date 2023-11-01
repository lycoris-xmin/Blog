using Lycoris.Blog.Application.AppServices.LoginFailedRecords.Dtos;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    public class LoginFailedRecordQueueModel
    {
        public LoginFailedRecordQueueModel() { }

        public LoginFailedRecordQueueModel(string Email, LoginFailedRecordDto data)
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
