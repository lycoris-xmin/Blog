namespace Lycoris.Blog.Application.Cached.LoginFailedRecord.Models
{
    public class LoginFailedRecordCacheModel
    {
        public LoginFailedRecordCacheModel() { }


        public LoginFailedRecordCacheModel(int Count, DateTime? FreezeTime)
        {
            this.Count = Count;
            this.FreezeTime = FreezeTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? FreezeTime { get; set; }
    }
}
