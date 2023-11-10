using Lycoris.Autofac.Extensions;

namespace Lycoris.Blog.Model.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ApplicationContext
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> AccessControl { get; set; } = new List<string>();
    }
}
