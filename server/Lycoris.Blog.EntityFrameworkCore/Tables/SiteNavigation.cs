using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站收录信息表
    /// </summary>
    [Table("Site_Navigation")]
    [TableIndex("Domain", true)]
    [TableIndex("Name")]
    [TableIndex("GroupId")]
    public class SiteNavigation : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 收录名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 收录地址
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Domain { get; set; } = "";

        /// <summary>
        /// 收录地址
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Url { get; set; } = "";

        /// <summary>
        /// 分组
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 收录热度
        /// </summary>
        [Comment("收录热度")]
        public int HrefCount { get; set; } = 0;

        /// <summary>
        /// 收录时间
        /// </summary>
        [Comment("收录时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 组内排序
        /// </summary>
        public int Order { get; set; }
    }
}
