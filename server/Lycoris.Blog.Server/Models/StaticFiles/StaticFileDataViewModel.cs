namespace Lycoris.Blog.Server.Models.StaticFiles
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticFileDataViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UploadChannel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PathUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RemoteUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? FileType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? FileSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? FileSha { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LocalBack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Use { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
