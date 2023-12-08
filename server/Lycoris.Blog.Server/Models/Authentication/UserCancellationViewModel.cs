namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class UserCancellationViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CancellationTime"></param>
        public UserCancellationViewModel(DateTime CancellationTime)
        {
            this.CancellationTime = CancellationTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CancellationTime { get; set; }
    }
}
