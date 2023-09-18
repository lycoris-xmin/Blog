using Castle.Core.Internal;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Model.Configurations;
using Microsoft.AspNetCore.Http;
using Minio;

namespace Lycoris.Blog.Core.CloudStorage.Minio.DataModel
{
    public class UploadFileArgsDataModel
    {
        private readonly MinioConfiguration Configuration;

        public UploadFileArgsDataModel(MinioConfiguration configuration)
        {
            Configuration = configuration;
            BucketName = Configuration.DefaultBucket ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        internal string BucketName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal string? ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string? FileName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal string? BucketSavePath { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal Stream? Stream { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, string>? Meta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BucketName"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithBucketName(string BucketName)
        {
            this.BucketName = BucketName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithBucketPath(string path)
        {
            BucketSavePath = path;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithFile(string filePath, string? uploadName = null)
        {
            FilePath = filePath;
            FileName = uploadName ?? Path.GetFileName(FilePath);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filestream"></param>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithFile(Stream filestream, string uploadName)
        {
            Stream = filestream;
            FileName = uploadName;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadName"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithFormFile(IFormFile file, string? uploadName = null)
        {
            Stream = file.OpenReadStream();
            FileName = uploadName ?? $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithMeta(params (string, string)[] values)
        {
            if (values.HasValue())
            {
                Meta ??= new Dictionary<string, string>();
                values.ForEach(x => Meta.TryAdd(x.Item1, x.Item2));
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public UploadFileArgsDataModel WithContentType(string ContentType)
        {
            this.ContentType = ContentType;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PutObjectArgs BuildPutObjectArgs()
        {
            var args = new PutObjectArgs();

            args.WithBucket(BucketName ?? Configuration.DefaultBucket);
            if (BucketSavePath.IsNullOrEmpty())
                args.WithObject(FileName);
            else
                args.WithObject($"{BucketSavePath!.TrimEnd('/').TrimEnd('\\')}/{FileName}");

            if (Stream != null)
            {
                args.WithStreamData(Stream);
                args.WithObjectSize(Stream.Length);
                ContentType = "application/octet-stream";
            }
            else
            {
                args.WithFileName(FilePath);
            }

            if (ContentType.IsNullOrEmpty())
                args.WithContentType(ContentType);

            if (BucketSavePath.IsNullOrEmpty())
                FileUrl = $"{Configuration.Endpoint}/{BucketName}/{FileName}";
            else
                FileUrl = $"{Configuration.Endpoint}/{BucketName}/{BucketSavePath!.TrimStart('/').TrimStart('\\').TrimEnd('/').TrimEnd('\\')}/{FileName}";

            return args;
        }
    }
}
