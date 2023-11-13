using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.EntityFrameworkCore.Repositories;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Contexts;

namespace Lycoris.Blog.Application.Shared.Impl
{
    public class ApplicationBaseService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        public Lazy<ILycorisLoggerFactory>? LycorisLoggerFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public Lazy<RequestContext> RequestContext { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public Lazy<IConfigurationRepository> ApplicationConfiguration { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 
        /// </summary>
        private ILycorisLogger? logger = null;

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable IDE1006 // 命名样式
        protected virtual ILycorisLogger _logger
#pragma warning restore IDE1006 // 命名样式
        {
            get
            {
                this.logger ??= LycorisLoggerFactory!.Value.CreateLogger(this.GetType());
                return this.logger;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual RequestContext CurrentRequest { get => this.RequestContext.Value; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual RequestUserContext CurrentUser { get => this.RequestContext.Value.User ?? new RequestUserContext(); }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool LoginState { get => CurrentUser.Id > 0; }

        /// <summary>
        /// 页码累计超过总数则不需要再去查询数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageFilter"></param>
        /// <param name="count"></param>
        /// <returns>True-未超过页码,False-超过页码</returns>
        protected virtual bool CheckPageFilter<T>(T pageFilter, int count) where T : PageFilter
        {
            if (pageFilter.PageSize <= 0)
                return true;

            var pageIndex = (int)Math.Ceiling((double)count / pageFilter.PageSize);
            return pageIndex >= pageFilter.PageIndex;
        }

        /// <summary>
        /// 页码累计超过总数则不需要再去查询数据库
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual bool CheckPageFilter(int pageIndex, int pageSize, int count)
        {
            if (pageIndex <= 0 || pageSize <= 0)
                return true;

            return (int)Math.Ceiling((double)count / pageSize) >= pageIndex;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class RequestUserContextExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static UserStatusEnum GetUserStatus(this RequestUserContext context) => (UserStatusEnum)context.Status;
    }
}
