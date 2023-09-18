namespace Lycoris.Blog.EntityFrameworkCore.Tables.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum FileSaveChannelEnum
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local = 0,
        /// <summary>
        /// Minio
        /// </summary>
        Minio = 10,
        /// <summary>
        /// 阿里云存储
        /// </summary>
        OSS = 20,
        /// <summary>
        /// 腾讯云存储
        /// </summary>
        COS = 30,
        /// <summary>
        /// 华为云存储
        /// </summary>
        OBS = 40,
        /// <summary>
        /// 七牛云存储
        /// </summary>
        Kodo = 50
    }
}
