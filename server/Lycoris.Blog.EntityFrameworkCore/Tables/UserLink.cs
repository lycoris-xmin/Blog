using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 用户三方绑定信息
    /// </summary>
    [Table("User_Link")]
    public class UserLink : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 主键、用户编号
        /// </summary>
        [TableColumn(IsPrimary = true)]
        public override long Id { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [TableColumn(StringLength = 100, Sensitive = true)]
        public string QQ { get; set; } = "";

        /// <summary>
        /// 微信
        /// </summary>
        [TableColumn(StringLength = 100, Sensitive = true)]
        public string WeChat { get; set; } = "";

        /// <summary>
        /// Github
        /// </summary>
        [TableColumn(StringLength = 255, Sensitive = true)]
        public string Github { get; set; } = "";

        /// <summary>
        /// 网易云
        /// </summary>
        [TableColumn(StringLength = 255, Sensitive = true)]
        public string CloudMusic { get; set; } = "";

        /// <summary>
        /// 哔哩哔哩
        /// </summary>
        [TableColumn(StringLength = 255, Sensitive = true)]
        public string Bilibili { get; set; } = "";

        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData()
        {
            return new List<object>()
            {
                new UserLink()
                {
                    Id = TableSeedData.UserData.Id,
                    QQ = "963770856",
                    WeChat = "",
                    Github = "https://github.com/lycoris-xmin",
                    CloudMusic = "",
                    Bilibili = ""
                }
            };
        }
    }
}
