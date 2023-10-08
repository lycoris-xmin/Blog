using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Core.CloudStorage.Minio.DataModel;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.Exceptions;

namespace Lycoris.Blog.Core.CloudStorage.Minio.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class MinioService : IMinioService
    {
        private readonly IMinioFactory _factory;
        private readonly IRepository<Configuration, int> _congiguration;

        public MinioService(IMinioFactory factory, IRepository<Configuration, int> congiguration)
        {
            _factory = factory;
            _congiguration = congiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            var config = await GetMinioConfigurationAsync();
            var minio = _factory.CreateClient(config);
            return await minio!.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task CreateBucketAsync(string bucketName)
        {
            var config = await GetMinioConfigurationAsync();
            var minio = _factory.CreateClient(config);
            await minio!.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<BucketDataModel>> GetBucketListAsync()
        {
            var config = await GetMinioConfigurationAsync();
            var minio = _factory.CreateClient(config);
            var list = await minio.ListBucketsAsync().ConfigureAwait(false);
            return list.Buckets.Select(x => new BucketDataModel() { BucketName = x.Name, CreateTime = x.CreationDateDateTime }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task RemoveBucketAsync(string bucketName)
        {
            var config = await GetMinioConfigurationAsync();
            var minio = _factory.CreateClient(config);
            await minio!.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(Action<UploadFileArgsDataModel> configre)
        {
            try
            {
                var config = await GetMinioConfigurationAsync();

                var args = new UploadFileArgsDataModel(config);
                configre.Invoke(args);

                var minioArgs = args.BuildPutObjectArgs();

                var minio = _factory.CreateClient(config);

                await minio.PutObjectAsync(minioArgs).ConfigureAwait(false);

                return args.FileUrl;
            }
            catch (ConnectionException)
            {
                throw new FriendlyException("上传文件失败,请检查Minio配置");
            }
            catch (Exception ex)
            {
                throw new FriendlyException("上传文件失败", ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RemoveFileAsync(Action<RemoveFileArgsDataModel> configure)
        {
            try
            {
                var config = await GetMinioConfigurationAsync();

                var args = new RemoveFileArgsDataModel(config);
                configure.Invoke(args);

                var minioArgs = args.BuildRemoveObjectArgs();

                var minio = _factory.CreateClient(config);

                await minio.RemoveObjectAsync(minioArgs).ConfigureAwait(false);
            }
            catch (ConnectionException)
            {
                throw new FriendlyException("删除文件失败,请检查Minio配置");
            }
            catch (Exception ex)
            {
                throw new FriendlyException("删除文件失败", ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public async Task<string> GetPresignedFileUrlAsync(Action<GetRandomFileUrlArgsDataModel> configure)
        {
            try
            {
                var config = await GetMinioConfigurationAsync();

                var args = new GetRandomFileUrlArgsDataModel(config);
                configure.Invoke(args);

                var minioArgs = args.BuildPresignedGetObjectArgs();

                var minio = _factory.CreateClient(config);

                return await minio.PresignedGetObjectAsync(minioArgs);
            }
            catch (ConnectionException)
            {
                throw new FriendlyException("生成访问地址失败,请检查Minio配置");
            }
            catch (Exception ex)
            {
                throw new FriendlyException("生成访问地址失败", ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<MinioConfiguration> GetMinioConfigurationAsync()
        {
            var data = await _congiguration.GetAll().Where(x => x.ConfigId == AppConfig.FileUpload).SingleOrDefaultAsync();

            if (data == null || data.Value.IsNullOrEmpty())
                throw new ArgumentNullException($"can not find setting with '{AppConfig.FileUpload}'");

            var config = data.Value!.ToObject<FileUploadConfiguration>();

            if (config == null || config.SaveChannel != FileSaveChannelEnum.Minio || config.Minio.MinioEndpoint.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(config));

            return config.Minio;
        }
    }
}
