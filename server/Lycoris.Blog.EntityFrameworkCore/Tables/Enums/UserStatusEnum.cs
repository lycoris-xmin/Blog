using System.ComponentModel;

namespace Lycoris.Blog.EntityFrameworkCore.Tables.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum UserStatusEnum
    {
        /// <summary>
        /// 未审核
        /// </summary>
        [Description("未审核")]
        Defalut = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 1,
        /// <summary>
        /// 黑名单
        /// </summary>
        [Description("黑名单")]
        Black = -1,
        /// <summary>
        /// 帐号注销
        /// </summary>
        [Description("帐号注销")]
        Cancellation = 100
    }
}
