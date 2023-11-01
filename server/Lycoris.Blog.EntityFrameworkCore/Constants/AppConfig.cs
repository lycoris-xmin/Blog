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
        public const string WebSettings = "App.WebSettings";

        /// <summary>
        /// 博客设置
        /// </summary>
        [Configuration("博客设置", typeof(PostSettingConfiguration))]
        public const string PostSettings = "App.PostSettings";

        /// <summary>
        /// 邮件服务设置
        /// </summary>
        [Configuration("邮件服务设置", typeof(EmailSettingsConfiguration))]
        public const string EmailSettings = "App.EmailSettings";

        /// <summary>
        /// SEO设置
        /// </summary>
        [Configuration("SEO设置", typeof(SeoSettingsConfiguration))]
        public const string SeoSettings = "App.SeoSettings";

        /// <summary>
        /// 文件上传设置
        /// </summary>
        [Configuration("SEO设置", typeof(FileUploadConfiguration))]
        public const string FileUpload = "App.FileUpload";

        /// <summary>
        /// 
        /// </summary>
        [Configuration("网站数据统计", typeof(WebStatisticsConfiguration))]
        public const string WebStatistics = "App.WebStatistics";

        /// <summary>
        /// 其他设置
        /// </summary>
        [Configuration("其他设置", typeof(OtherSettingsConfiguration))]
        public const string OtherSettings = "App.OtherSettings";
    }
}
