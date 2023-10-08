using Lycoris.Blog.Server.PropertyAttribute;

namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class UnLockInput
    {
        /// <summary>
        /// 
        /// </summary>
        [PasswordValid(Required = true)]
        public string? Password { get; set; }
    }
}
