using Lycoris.Base.Extensions;
using Lycoris.Blog.Model.Configurations;
using Minio;

namespace Lycoris.Blog.Core.CloudStorage.Minio.DataModel
{
    public class GetRandomFileUrlArgsDataModel
    {
        public GetRandomFileUrlArgsDataModel(MinioConfiguration configuration)
        {
            BucketName = configuration.DefaultBucket ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        internal string BucketName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal IDictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal DateTime? RequestDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal int ExpiresInSecs { get; set; } = 600;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BucketName"></param>
        /// <returns></returns>
        public GetRandomFileUrlArgsDataModel WithBucketName(string BucketName)
        {
            this.BucketName = BucketName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GetRandomFileUrlArgsDataModel WithBucketPath(string path)
        {
            FilePath = path;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RequestDate"></param>
        /// <returns></returns>
        public GetRandomFileUrlArgsDataModel WithRequestDate(DateTime RequestDate)
        {
            this.RequestDate = RequestDate;
            return this;
        }

        /// <summary>
        /// 设置有效期（以秒为单位）
        /// </summary>
        /// <param name="ExpiresInSecs"></param>
        /// <returns></returns>
        public GetRandomFileUrlArgsDataModel WithExpiresInSecs(int ExpiresInSecs)
        {
            this.ExpiresInSecs = ExpiresInSecs;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GetRandomFileUrlArgsDataModel WithHeaders(IDictionary<string, string> Headers)
        {
            this.Headers = Headers;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal PresignedGetObjectArgs BuildPresignedGetObjectArgs()
        {
            var args = new PresignedGetObjectArgs();

            args.WithBucket(BucketName);
            args.WithObject(FilePath);
            args.WithExpiry(ExpiresInSecs);

            if (Headers.HasValue())
                args.WithHeaders(Headers);

            if (RequestDate.HasValue)
                args.WithRequestDate(RequestDate);

            return args;
        }
    }
}
