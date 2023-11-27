using Lycoris.Blog.Model.Contexts;
using Newtonsoft.Json;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class LoginRecordQueueModel
    {
        public LoginRecordQueueModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Context"></param>
        public LoginRecordQueueModel(long UserId, RequestContext Context)
        {
            this.UserId = UserId;
            this.UserAgent = Context.UserAgent;
            this.Ip = Context.RequestIP;
            this.Success = true;
            this.Remark = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Context"></param>
        /// <param name="Remark"></param>
        public LoginRecordQueueModel(long UserId, RequestContext Context, string Remark)
        {
            this.UserId = UserId;
            this.UserAgent = Context.UserAgent;
            this.Ip = Context.RequestIP;
            this.Remark = Remark;
            this.Success = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Context"></param>
        /// <param name="Success"></param>
        /// <param name="Remark"></param>
        public LoginRecordQueueModel(long UserId, RequestContext Context, bool Success, string Remark)
        {
            this.UserId = UserId;
            this.UserAgent = Context.UserAgent;
            this.Ip = Context.RequestIP;
            this.Success = Success;
            this.Remark = Remark;
        }


        public long UserId { get; set; }

        public string UserAgent { get; set; } = "";

        public string Ip { get; set; } = "";

        public bool Success { get; set; } = true;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Remark { get; set; }
    }
}
