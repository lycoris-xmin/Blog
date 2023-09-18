namespace Lycoris.Blog.Server.Models.LeaveMessages
{
    /// <summary>
    /// 
    /// </summary>
    public class LeaveMessageQueryDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? OriginalContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? CreateUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOwner { get; set; }
    }
}
