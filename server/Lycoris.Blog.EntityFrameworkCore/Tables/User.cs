using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [Table("User")]
    [TableIndex("Email", true)]
    [TableIndex(new[] { "Id", "NickName", "Avatar" })]
    public class User : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 帐号
        /// </summary>
        [TableColumn(StringLength = 100, Sensitive = true, Required = true)]
        public string Email { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        [TableColumn(StringLength = 255, Required = true, SqlPassword = true)]
        public string Password { get; set; } = "";

        /// <summary>
        /// 用户昵称
        /// </summary>
        [TableColumn(StringLength = 30)]
        public string NickName { get; set; } = "";

        /// <summary>
        /// 头像
        /// </summary>
        [TableColumn(StringLength = 255)]
        public string Avatar { get; set; } = "";

        /// <summary>
        /// 用户状态
        /// </summary>
        [TableColumn(Required = true, DefaultValue = UserStatusEnum.Defalut)]
        public UserStatusEnum Status { get; set; }

        /// <summary>
        /// 是否为网站管理员
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 是否开启二次认证
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool GoogleAuthentication { get; set; } = false;

        /// <summary>
        /// 是否展示在线状态
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool ShowOnlineStatus { get; set; } = false;

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 用户备注
        /// </summary>
        [TableColumn(StringLength = 500, Required = false)]
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData()
        {
            return new List<object>()
            {
                new User()
                {
                    Id = TableSeedData.UserData.Id,
                    Email = AppSettings.Sql.SeedData.Email,
                    Password = AppSettings.Sql.SeedData.Password,
                    NickName = AppSettings.Sql.SeedData.NickName,
                    Avatar = AppSettings.Sql.SeedData.DefaultAvatar,
                    IsAdmin = true,
                    GoogleAuthentication = false,
                    ShowOnlineStatus = true,
                    Status = UserStatusEnum.Audited,
                    CreateTime  = new DateTime(2020,1,1)
                }
            };
        }
    }
}
