using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Minio;

namespace Lycoris.Blog.Core.Minio.Impl
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
                _client.WithEndpoint(configuration!.MinioEndpoint).WithCredentials(configuration!.AccessKey, configuration!.SecretKey).WithSSL(configuration!.SSL).Build();
            }

            return _client;
        }
    }
}
