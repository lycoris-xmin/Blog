using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.EntityFrameworkCore.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 网站设置
        /// </summary>
        [Configuration("网站设置", typeof(WebSettingsConfiguration))]
        public const string WebSetting = "App.WebSettings";

        /// <summary>
        /// 博客设置
        /// </summary>
        [Configuration("博客设置", typeof(PostSettingConfiguration))]
        public const string PostSetting = "App.PostSettings";

        /// <summary>
        /// 邮件服务设置
        /// </summary>
        [Configuration("邮件服务设置", typeof(EmailSettingsConfiguration))]
        public const string EmailSetting = "App.EmailSettings";

        /// <summary>
        /// SEO设置
        /// </summary>
        [Configuration("SEO设置", typeof(SeoSettingsConfiguration))]
        public const string SeoSetting = "App.SeoSettings";

        /// <summary>
        /// 静态文件设置
        /// </summary>
        [Configuration("静态文件设置", typeof(StaticFileConfiguration))]
        public const string StaticFile = "App.StaticFile";

        /// <summary>
        /// 
        /// </summary>
        [Configuration("网站数据统计", typeof(WebStatisticsConfiguration))]
        public const string WebStatistics = "App.WebStatistics";

        /// <summary>
        /// 系统设置
        /// </summary>
        [Configuration("系统设置", typeof(SystemSettingsConfiguration))]
        public const string SystemSetting = "App.SystemSettings";
    }
}
