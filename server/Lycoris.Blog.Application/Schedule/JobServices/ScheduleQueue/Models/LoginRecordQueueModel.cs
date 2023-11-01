﻿using Lycoris.Blog.Model.Contexts;
using Newtonsoft.Json;

namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    public class LoginRecordQueueModel
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
            UserAgent = Context.UserAgent;
            Ip = Context.RequestIP;
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
            UserAgent = Context.UserAgent;
            Ip = Context.RequestIP;
            this.Remark = Remark;
        }

        public long UserId { get; set; }

        public string UserAgent { get; set; } = "";

        public string Ip { get; set; } = "";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Remark { get; set; }
    }
}