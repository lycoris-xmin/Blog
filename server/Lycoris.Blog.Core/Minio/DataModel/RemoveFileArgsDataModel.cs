using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Minio;

namespace Lycoris.Blog.Core.Minio.DataModel
{
    public class RemoveFileArgsDataModel
    {
        private readonly MinioConfiguration Configuration;

        public RemoveFileArgsDataModel(MinioConfiguration configuration)
        {
            Configuration = configuration;
            BucketName = Configuration.Bucket ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        internal string BucketName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BucketName"></param>
        /// <returns></returns>
        public RemoveFileArgsDataModel WithBucketName(string BucketName)
        {
            this.BucketName = BucketName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public RemoveFileArgsDataModel WithBucketPath(string path)
        {
            FilePath = path;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public RemoveFileArgsDataModel WithFileUrl(string fileUrl)
        {
            if (fileUrl.StartsWith(Configuration.Endpoint!))
                fileUrl = fileUrl.Replace(Configuration.Endpoint!, "").Replace(BucketName, "").TrimStart('/');

            FileName = $"/{fileUrl}";
            FilePath = string.Empty;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal RemoveObjectArgs BuildRemoveObjectArgs()
        {
            var objectName = FileName;
            if (!FilePath.IsNullOrEmpty())
                objectName = $"{FilePath.TrimEnd('/')}/{FileName.TrimStart('/')}";

            if (objectName.StartsWith(Configuration.Endpoint!))
                objectName = objectName.Replace(Configuration.Endpoint!, "");

            if (objectName.StartsWith(BucketName))
                objectName = objectName.Replace(BucketName, "").TrimStart('/');

            return new RemoveObjectArgs().WithBucket(BucketName).WithObject($"/{objectName}");
        }
    }
}
