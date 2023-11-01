using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 友情链接
    /// </summary>
    [Table("FriendLink")]
    [TableIndex("Status")]
    [TableIndex("CreateUserId")]
    [TableIndex("CreateTime")]
    public class FriendLink : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 头像链接
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Icon { get; set; } = "";

        /// <summary>
        /// 网站链接
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Link { get; set; } = "";

        /// <summary>
        /// 网站介绍
        /// </summary>
        [TableColumn(StringLength = 300)]
        public string Description { get; set; } = "";

        /// <summary>
        /// 审核状态
        /// </summary>
        [TableColumn(ColumnType = MySqlType.TINYINT, DefaultValue = FriendLinkStatusEnum.Default)]
        public FriendLinkStatusEnum Status { get; set; }

        /// <summary>
        /// 申请用户编号
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 链接跳转次数
        /// </summary>
        public int RouteLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [TableColumn(DefaultValue = "")]
        public string Remark { get; set; } = "";
    }
}
