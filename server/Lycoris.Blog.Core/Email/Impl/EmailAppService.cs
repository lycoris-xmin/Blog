using HtmlAgilityPack;
using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core.Email.DataModel;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Common.Extensions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace Lycoris.Blog.Core.Email.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class EmailAppService : IEmailAppService
    {
        private readonly IConfigurationRepository _configuration;

        public EmailAppService(IConfigurationRepository configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 测试邮件(主要用来管理员测试邮箱配置是否正确)
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task SendTestEmailAsync(string emailAddress)
        {
            var (option, basic) = await GetAllConfigurationAsync();
            await SendTestEmailAsync(emailAddress, option, basic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="option"></param>
        /// <param name="basic"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task SendTestEmailAsync(string emailAddress, EmailSettingsConfiguration option, WebSettingsConfiguration? basic = null)
        {
            try
            {
                var message = new MimeMessage();
                //发件人
                message.From.Add(new MailboxAddress(option!.EmailUser, option.EmailAddress));
                //接收人
                message.To.Add(new MailboxAddress("", emailAddress));
                //邮件标题
                message.Subject = $"[{option.EmailSignature}] 邮件服务测试";

                if (!File.Exists(AppSettings.Path.EmailTemplate.EmailTest))
                    throw new FriendlyException("邮件服务测试模版文件不存在");

                var doc = new HtmlDocument();
                doc.Load(AppSettings.Path.EmailTemplate.EmailTest);

                doc.GetElementbyId("emailUser").InnerHtml = option.EmailSignature;
                if (basic != null)
                {
                    doc.GetElementbyId("web-home-link").SetAttributeValue("href", basic.WebPath);
                    doc.GetElementbyId("web-home").InnerHtml = basic.WebName;
                }

                string htmlStr = "";
                using (var sr = new MemoryStream())
                {
                    doc.Save(sr);
                    var b = sr.ToArray();
                    htmlStr = Encoding.UTF8.GetString(b, 0, b.Length);
                }

                var body = new TextPart(TextFormat.Html) { Text = htmlStr };

                // 创建Multipart添加附件
                var multipart = new Multipart("mixed") { body };

                // 正文内容，发送
                message.Body = multipart;

                await SendAsync(option, message);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("535"))
                    throw new FriendlyException("发件箱地址或密码错误", ex.Message);

                throw new FriendlyException("测试邮件发送失败", ex.Message);
            }
        }

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendEmailCaptchaAsync(SendEmailCaptchaDataModel input)
        {
            var (option, basic) = await GetAllConfigurationAsync();

            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(option.EmailUser, option.EmailAddress));

                //接收人
                message.To.Add(new MailboxAddress(input.NickName, input.EmailAddress));

                //邮件标题
                message.Subject = $"[{basic.WebName}] 邮箱验证码";

                if (!File.Exists(AppSettings.Path.EmailTemplate.EmailValidate))
                    throw new FriendlyException("邮件服务验证码模版文件不存在");

                var doc = new HtmlDocument();
                doc.Load(AppSettings.Path.EmailTemplate.EmailValidate);

                if (!input.NickName.IsNullOrEmpty())
                    doc.GetElementbyId("nickName").InnerHtml = input.NickName;

                doc.GetElementbyId("actionName").InnerHtml = input.Action ?? "验证码";
                doc.GetElementbyId("captcha").InnerHtml = input.Code;
                doc.GetElementbyId("emailUser").InnerHtml = option.EmailSignature;

                if (input.ExpireTime.HasValue && input.ExpireTime.Value > 0)
                    doc.GetElementbyId("expireTime").InnerHtml = input.ExpireTime.Value.ToString();

                doc.GetElementbyId("web-home-link").SetAttributeValue("href", basic.WebPath);
                doc.GetElementbyId("web-home").InnerHtml = basic.WebName;


                string htmlStr = "";
                using (var sr = new MemoryStream())
                {
                    doc.Save(sr);
                    var b = sr.ToArray();
                    htmlStr = Encoding.UTF8.GetString(b, 0, b.Length);
                }

                var body = new TextPart(TextFormat.Html) { Text = htmlStr };

                // 创建Multipart添加附件
                var multipart = new Multipart("mixed") { body };

                // 正文内容，发送
                message.Body = multipart;

                await SendAsync(option, message);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("535"))
                    throw new FriendlyException("邮件发送失败", "发件箱地址或密码错误");

                throw new FriendlyException("邮件发送失败", ex.Message);
            }
        }

        /// <summary>
        /// 重置密码邮件
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="emailValidId"></param>
        /// <returns></returns>
        public async Task SendResetPasswordEmailAsync(string nickName, string emailAddress, long emailValidId)
        {
            var (option, basic) = await GetAllConfigurationAsync();

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(option.EmailUser, option.EmailAddress));

                //接收人
                message.To.Add(new MailboxAddress(nickName, emailAddress));

                //邮件标题
                message.Subject = $"[{basic.WebName}] 重置密码";

                if (!File.Exists(AppSettings.Path.EmailTemplate.ResetPassword))
                    throw new FriendlyException("邮件服务激活模版文件不存在");

                var doc = new HtmlDocument();
                doc.Load(AppSettings.Path.EmailTemplate.ResetPassword);

                doc.GetElementbyId("nickName").InnerHtml = nickName;
                doc.GetElementbyId("emailUser").InnerHtml = option.EmailSignature;

                var link = $"{basic.WebPath}/Authorize/ResetPassword/{emailValidId}";

                doc.GetElementbyId("link").SetAttributeValue("href", link);
                doc.GetElementbyId("btn-link").SetAttributeValue("href", link);
                doc.GetElementbyId("web-home-link").SetAttributeValue("href", basic.WebPath);
                doc.GetElementbyId("web-home").InnerHtml = basic.WebName;

                string htmlStr = "";
                using (var sr = new MemoryStream())
                {
                    doc.Save(sr);
                    var b = sr.ToArray();
                    htmlStr = Encoding.UTF8.GetString(b, 0, b.Length);
                }

                var body = new TextPart(TextFormat.Html) { Text = htmlStr };

                // 创建Multipart添加附件
                var multipart = new Multipart("mixed") { body };

                // 正文内容，发送
                message.Body = multipart;

                await SendAsync(option, message);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("535"))
                    throw new FriendlyException("邮件发送失败", "发件箱地址或密码错误");

                throw new FriendlyException("邮件发送失败", ex.Message);
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendAsync(SendEmailDataModel input)
        {
            var option = await GetEmailConfigurationAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(option.EmailUser ?? "", option.EmailAddress));

            // 接收人
            input.ToUser.ForEach(x => message.To.Add(new MailboxAddress(x.UserName ?? "", x.Email)));

            // 邮件标题
            message.Subject = input.Title;

            // html正文

            var body = !input.Body.Html.IsNullOrEmpty()
                        ? new TextPart(TextFormat.Html) { Text = input.Body.Html }
                        : new TextPart(TextFormat.Text) { Text = input.Body.Text ?? "" };

            // 创建Multipart添加附件
            var multipart = new Multipart("mixed") { body };

            if (input.MultipartPath.HasValue())
            {
                input.MultipartPath.ForEach(x =>
                {
                    var attachment = new MimePart()
                    {
                        // 读取文件,只能用绝对路径
                        Content = new MimeContent(File.OpenRead(x), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        // 文件名字
                        FileName = Path.GetFileName(x)
                    };

                    multipart.Add(attachment);
                });
            }

            // 设置正文内容
            message.Body = multipart;

            await SendAsync(option, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private async Task<EmailSettingsConfiguration> GetEmailConfigurationAsync()
        {
            var option = await _configuration.GetConfigurationAsync<EmailSettingsConfiguration>(AppConfig.EmailSettings)
                ?? throw new FriendlyException("邮件服务暂时不能使用,请稍候再试", "未获取到有效邮件服务配置");

            // option 验证

            return option;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private async Task<(EmailSettingsConfiguration option, WebSettingsConfiguration basic)> GetAllConfigurationAsync()
        {
            var option = await GetEmailConfigurationAsync();

            var basic = await _configuration.GetConfigurationAsync<WebSettingsConfiguration>(AppConfig.WebSettings)
                ?? throw new FriendlyException("邮件服务暂时不能使用,请稍候再试", $"未获取到有效网站域名配置,无法发送邮箱认证邮件");

            return (option, basic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task SendAsync(EmailSettingsConfiguration option, MimeMessage message)
        {
            using var client = new SmtpClient();

            // Smtp服务器
            if (option.UseSSL)
                await client.ConnectAsync(option.STMPServer, option.STMPPort, SecureSocketOptions.StartTls);
            else
                await client.ConnectAsync(option.STMPServer, option.STMPPort);

            // 登录,发送
            // 特别说明,对于服务器端的中文相应,Exception中有编码问题,显示乱码了
            // 发件人的邮箱账号密码
            client.Authenticate(option.EmailAddress, option.EmailPassword);

            await client.SendAsync(message);

            // 断开
            client.Disconnect(true);
        }
    }
}
