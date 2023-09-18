using Lycoris.Base.Extensions;
using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.Model.Configurations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站配置表
    /// </summary>
    [Table("Configuration")]
    [TableIndex("ConfigId", true)]
    [TableIndex("ConfigName")]
    public class Configuration : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 配置标识
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string ConfigId { get; set; } = string.Empty;

        /// <summary>
        /// 配置名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string ConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 配置值
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<object> InitialData()
        {
            var id = 1;
            return new List<object>()
            {
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.WebSettings,
                    ConfigName = "网站设置",
                    Value = new WebSettingsConfiguration()
                    {
                        WebName = "程序猿的小破站",
                        WebPath = "https://lycoris.cloud",
                        AdminPath = "https://xmin.lycoris.cloud/little/brokenstation/login",
                        BuildTime = new DateTime(2018,5,18)
                    }.ToJson()
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.PostSettings,
                     ConfigName = "博客设置",
                    Value = new PostSettingConfiguration().ToJson()
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.EmailSettings,
                    ConfigName = "邮件服务设置",
                    Value = new EmailSettingsConfiguration()
                    {
                        EmailAddress = "zzyo.yj.min@qq.com",
                        EmailPassword = "fvvalihjdgigbecg",
                        EmailUser = "Lycoris",
                        EmailSignature  = "程序猿的小破站",
                        STMPServer = "smtp.qq.com",
                        STMPPort = 25,
                        UseSSL= true
                    }.ToJson()
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.SeoSettings,
                    ConfigName = "SEO设置",
                    Value = new SeoSettingsConfiguration()
                    {
                         Biadu = new BaiduSeoConfiguration()
                         {
                            Enabled = false,
                            Host = "http://data.zz.baidu.com/urls",
                            Site = "https://www.lycoris.cloud",
                            Token = "m6RmIsnTJoH8nMTc"
                         }
                    }.ToJson()
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.FileUpload,
                    ConfigName = "文件上传设置",
                    Value = new FileUploadConfiguration()
                    {
                        Minio = new MinioConfiguration()
                        {
                            Endpoint = "http://119.23.78.111:8005",
                            AccessKey = "HrXIRHCjU9xJBaYb",
                            SecretKey = "pf6tU5Y2MAjkBKuuQIIMVj7w7cJMTSpJ",
                            SSL = false,
                            DefaultBucket ="lycoris"
                        }
                    }.ToJson()
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.OtherSettings,
                    ConfigName = "Showdoc推送设置",
                    Value = new OtherSettingsConfiguration()
                    {
                        ShowDocHost =  "https://push.showdoc.com.cn/server/api/push/6f01df770ce1e4617d5785c98ed0d9841156708971"
                    }.ToJson()
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.AboutWeb,
                    ConfigName = "关于本站",
                    Value = ""
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.AboutMeInfo,
                    ConfigName = "关于我 - 基础信息",
                    Value = ""
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.AboutMeSkill,
                    ConfigName = "关于我 - 掌握技术",
                    Value = ""
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.AboutMeProject,
                    ConfigName = "关于我 - 项目经验",
                    Value = ""
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.AboutMeOffice,
                    ConfigName = "关于我 - 工作经历",
                    Value = ""
                },
                new Configuration()
                {
                    Id = id++,
                    ConfigId = AppConfig.WebStatistics,
                    ConfigName = "网站数据统计",
                    Value = new WebStatisticsConfiguration().ToJson()
                }
            };
        }
    }
}
