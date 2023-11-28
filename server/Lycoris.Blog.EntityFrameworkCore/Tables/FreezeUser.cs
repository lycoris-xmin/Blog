using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 冻结用户信息
    /// </summary>
    [Table("FreezeUser")]
    public class FreezeUser : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [Key]
        public override long Id { get; set; }

        /// <summary>
        /// 冻结开始时间
        /// </summary>
        public DateTime FreeBeginTime { get; set; }

        /// <summary>
        /// 冻结结束时间
        /// </summary>
        public DateTime FreeEndTime { get; set; }
    }
}
