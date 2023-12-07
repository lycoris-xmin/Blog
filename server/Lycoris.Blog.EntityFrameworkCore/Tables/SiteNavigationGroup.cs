using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 收录分组
    /// </summary>
    [Table("Site_Navigation_Group")]
    [TableIndex("GroupName", true)]
    [TableIndex("Order")]
    public class SiteNavigationGroup : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 收录分组
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 分组排序
        /// </summary>
        public int Order { get; set; }
    }
}
