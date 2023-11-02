using Lycoris.Blog.Core.Minio.DataModel;

namespace Lycoris.Blog.Core.Minio
{
    public interface IMinioService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task<bool> BucketExistsAsync(string bucketName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task CreateBucketAsync(string bucketName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<BucketDataModel>> GetBucketListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task RemoveBucketAsync(string bucketName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> UploadFileAsync(Action<UploadFileArgsDataModel> configre);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task RemoveFileAsync(Action<RemoveFileArgsDataModel> configure);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<string> GetPresignedFileUrlAsync(Action<GetRandomFileUrlArgsDataModel> configure);
    }
}
