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
        public const string WebSetting = "App.WebSetting";

        /// <summary>
        /// 文章设置
        /// </summary>
        [Configuration("文章设置", typeof(PostSettingConfiguration))]
        public const string PostSetting = "App.PostSetting";

        /// <summary>
        /// 留言设置
        /// </summary>
        [Configuration("留言设置", typeof(PostSettingConfiguration))]
        public const string MessageSetting = "App.MessageSetting";

        /// <summary>
        /// 邮件设置
        /// </summary>
        [Configuration("邮件设置", typeof(EmailSettingsConfiguration))]
        public const string EmailSetting = "App.EmailSetting";

        /// <summary>
        /// SEO设置
        /// </summary>
        [Configuration("SEO设置", typeof(SeoSettingsConfiguration))]
        public const string SeoSetting = "App.SeoSetting";

        /// <summary>
        /// 上传设置
        /// </summary>
        [Configuration("上传设置", typeof(UploadConfiguration))]
        public const string Upload = "App.UploadSetting";

        /// <summary>
        /// Showdoc推送设置
        /// </summary>
        [Configuration("Showdoc推送设置", typeof(ShowdocSettingsConfiguration))]
        public const string Showdoc = "App.Showdoc";

        /// <summary>
        /// 
        /// </summary>
        [Configuration("网站数据统计", typeof(WebStatisticsConfiguration))]
        public const string WebStatistics = "App.WebStatistic";

        /// <summary>
        /// 其他设置
        /// </summary>
        [Configuration("其他设置", typeof(OtherSettingsConfiguration))]
        public const string OtherSetting = "App.OtherSetting";
    }
}
