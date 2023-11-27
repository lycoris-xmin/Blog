namespace Lycoris.Blog.Server.Models.LoginRecords
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginRecordDataViewModel
    {
        /// <summary>
        /// UA
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 登录IP归属地
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登录状态
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
