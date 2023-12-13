namespace Lycoris.Blog.Server.Models.StaticFiles
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticFileUploadViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="FileType"></param>
        public StaticFileUploadViewModel(string? Url, int FileType)
        {
            this.Url = Url;
            this.FileType = FileType;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FileType { get; set; }
    }
}
