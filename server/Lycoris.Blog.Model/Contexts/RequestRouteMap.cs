using Lycoris.Autofac.Extensions;

namespace Lycoris.Blog.Model.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton, Self = true)]
    public class RequestRouteMap
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> WebRoute { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public List<string> DashboardRoute { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public RequestLogFilterEnum LogFilter { get; set; } = RequestLogFilterEnum.OnlyWeb;
    }


    /// <summary>
    /// 
    /// </summary>
    public enum RequestLogFilterEnum
    {
        /// <summary>
        /// 
        /// </summary>
        All = 0,
        /// <summary>
        /// 
        /// </summary>
        OnlyWeb = 1,
        /// <summary>
        /// 
        /// </summary>
        OnlyDashboard = 2
    }
}
