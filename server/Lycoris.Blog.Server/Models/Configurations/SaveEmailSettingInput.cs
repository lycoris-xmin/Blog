namespace Lycoris.Blog.Server.Models.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveEmailSettingInput
    {
        /// <summary>
        /// 发件邮箱地址
        /// </summary>
        public string? EmailAddress { get; set; }

        /// <summary>
        /// 发件人名称
        /// </summary>
        public string? EmailUser { get; set; }

        /// <summary>
        /// STMP服务器
        /// </summary>
        public string? STMPServer { get; set; }

        /// <summary>
        /// SMTP端口
        /// </summary>
        public int? STMPPort { get; set; }

        /// <summary>
        /// 发件邮箱密码
        /// </summary>
        public string? EmailPassword { get; set; }

        /// <summary>
        /// 邮件服务商署名
        /// </summary>
        public string? EmailSignature { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? UseSSL { get; set; }
    }
}
