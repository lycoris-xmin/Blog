using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Model.Configurations;
using Minio;

namespace Lycoris.Blog.Core.CloudStorage.Minio.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class MinioFactory : IMinioFactory
    {
        private string _orign = "";
        private MinioClient? _client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public MinioClient CreateClient(MinioConfiguration configuration)
        {
            var orign = SecretHelper.SHA256Encrypt(configuration.ToJson());
            if (_client == null || _orign != orign)
            {
                _orign = orign;
                _client = new MinioClient();
                _client.WithEndpoint(configuration!.MinioEndpoint).WithCredentials(configuration!.AccessKey, configuration!.SecretKey).WithSSL(configuration!.SSL ?? false).Build();
            }

            return _client;
        }
    }
}
