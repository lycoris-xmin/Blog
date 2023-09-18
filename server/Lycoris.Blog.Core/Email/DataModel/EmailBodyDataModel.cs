namespace Lycoris.Blog.Core.Email.DataModel
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailBodyDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        internal string? Html { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string? Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        public void SetHtmlBody(string html) => Html = html;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetTextBody(string text) => Text = text;
    }
}
