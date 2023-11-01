using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;

namespace Lycoris.Blog.EntityFrameworkCore.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public class AppAbout
    {
        /// <summary>
        /// 关于本站
        /// </summary>
        [Configuration("关于本站", "")]
        public const string AboutWeb = "App.AboutWeb";

        /// <summary>
        /// 关于我 - 基础信息
        /// </summary>
        [Configuration("关于我 - 基础信息", "")]
        public const string AboutMeInfo = "App.AboutMe.Info";

        /// <summary>
        /// 关于我 - 掌握技术
        /// </summary>
        [Configuration("关于我 - 掌握技术", "")]
        public const string AboutMeSkill = "App.AboutMe.Skill";

        /// <summary>
        /// 关于我 - 项目经验
        /// </summary>
        [Configuration("关于我 - 项目经验", "")]
        public const string AboutMeProject = "App.AboutMe.Project";

        /// <summary>
        /// 关于我 - 工作经历
        /// </summary>
        [Configuration("关于我 - 工作经历", "")]
        public const string AboutMeOffice = "App.AboutMe.Office";
    }
}
