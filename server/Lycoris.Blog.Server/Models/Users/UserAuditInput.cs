using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UserAuditInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public long? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int? Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Remark { get; set; }
    }
}
