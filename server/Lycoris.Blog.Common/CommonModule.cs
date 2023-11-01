using Lycoris.Autofac.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Blog.Common
{
    public class CommonModule : LycorisRegisterModule
    {
        public override void SerivceRegister(IServiceCollection services)
        {
            services.AddMemoryCache();
        }
    }
}