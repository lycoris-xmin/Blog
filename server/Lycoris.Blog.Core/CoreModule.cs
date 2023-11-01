using Lycoris.Autofac.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Blog.Core
{
    public class CoreModule : LycorisRegisterModule
    {
        public override void SerivceRegister(IServiceCollection services)
        {
            // 注册内存缓存
        }
    }
}