namespace Lycoris.Blog.Core.Email.DataModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SendEmailDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SendEmailDataModel()
        {
            ToUser = new List<EmailUserDataModel>();
            Body = new EmailBodyDataModel();
            MultipartPath = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToUser"></param>
        public SendEmailDataModel(params EmailUserDataModel[] ToUser)
        {
            this.ToUser = ToUser?.ToList() ?? new List<EmailUserDataModel>();
            Body = new EmailBodyDataModel();
            MultipartPath = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToUser"></param>
        public SendEmailDataModel(List<EmailUserDataModel> ToUser)
        {
            this.ToUser = ToUser?.ToList() ?? new List<EmailUserDataModel>();
            Body = new EmailBodyDataModel();
            MultipartPath = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToUser"></param>
        /// <param name="Body"></param>
        public SendEmailDataModel(List<EmailUserDataModel> ToUser, EmailBodyDataModel Body)
        {
            this.ToUser = ToUser?.ToList() ?? new List<EmailUserDataModel>();
            this.Body = Body;
            MultipartPath = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToUser"></param>
        /// <param name="Body"></param>
        /// <param name="MultipartPath"></param>
        public SendEmailDataModel(List<EmailUserDataModel> ToUser, EmailBodyDataModel Body, List<string> MultipartPath)
        {
            this.ToUser = ToUser?.ToList() ?? new List<EmailUserDataModel>();
            this.Body = Body;
            this.MultipartPath = MultipartPath;
        }

        /// <summary>
        /// 收件人
        /// </summary>
        public List<EmailUserDataModel> ToUser { get; set; }

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// html正文
        /// </summary>
        public EmailBodyDataModel Body { get; set; }

        /// <summary>
        /// 附件绝对路径
        /// </summary>
        public List<string> MultipartPath { get; set; }
    }
}
