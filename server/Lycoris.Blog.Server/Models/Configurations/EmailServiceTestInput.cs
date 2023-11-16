using System.ComponentModel.DataAnnotations;

namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailServiceTestInput
    {
        /// <summary>
        /// 发件邮箱地址
        /// </summary>
        [Required]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// 发件人名称
        /// </summary>
        [Required]
        public string? EmailUser { get; set; }

        /// <summary>
        /// STMP服务器
        /// </summary>
        [Required]
        public string? STMPServer { get; set; }

        /// <summary>
        /// SMTP端口
        /// </summary>
        [Required]
        public int? STMPPort { get; set; }

        /// <summary>
        /// 发件邮箱密码
        /// </summary>
        [Required]
        public string? EmailPassword { get; set; }

        /// <summary>
        /// 邮件服务商署名
        /// </summary>
        [Required]
        public string? EmailSignature { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool? UseSSL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? TestEmail { get; set; }
    }
}
