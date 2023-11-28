using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Blog.Model
{
    /// <summary>
    /// 模型注册模块
    /// </summary>
    public class ModelModule : LycorisRegisterModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public override void SerivceRegister(IServiceCollection services)
        {
            NewtonsoftJsonExtensions.SetGlobalJsonSerializerSetting(x => x.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace);
        }
    }
}