using Lycoris.Autofac.Extensions;
using Lycoris.Base.Logging;
using Lycoris.RabbitMQ.Extensions;

namespace Lycoris.Blog.Application.RqbbitMq.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class RabbitProducterService : IRabbitProducterService
    {
        private readonly ILycorisLogger _logger;
        private readonly IRabbitProducerFactory _rabbitFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="rabbitFactory"></param>
        public RabbitProducterService(ILycorisLoggerFactory factory, IRabbitProducerFactory rabbitFactory)
        {
            _logger = factory.CreateLogger<RabbitProducterService>();
            _rabbitFactory = rabbitFactory;
        }
    }
}
