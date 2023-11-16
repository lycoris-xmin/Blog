namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailSettingsConfiguration
    {
        /// <summary>
        /// 发件邮箱地址
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 发件人名称
        /// </summary>
        public string EmailUser { get; set; } = string.Empty;

        /// <summary>
        /// STMP服务器
        /// </summary>
        public string STMPServer { get; set; } = string.Empty;

        /// <summary>
        /// SMTP端口
        /// </summary>
        public int STMPPort { get; set; } = 0;

        /// <summary>
        /// 发件邮箱密码
        /// </summary>
        public string EmailPassword { get; set; } = string.Empty;

        /// <summary>
        /// 邮件服务商署名
        /// </summary>
        public string EmailSignature { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool UseSSL { get; set; } = true;
    }
}
