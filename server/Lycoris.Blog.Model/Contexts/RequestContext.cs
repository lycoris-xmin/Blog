﻿using Lycoris.Autofac.Extensions;

namespace Lycoris.Blog.Model.Contexts
{
    /// <summary>
    /// 请求上下文
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, Self = true)]
    public class RequestContext
    {
        /// <summary>
        /// 请求方唯一标识码
        /// </summary>
        public string TraceId { get; set; } = string.Empty;

        /// <summary>
        /// 请求方地址
        /// </summary>
        public string RequestIP { get; set; } = string.Empty;

        /// <summary>
        /// 访问客户端
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RequestUserContext? User { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestUserContext
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long Id { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdmin { get; set; } = false;
    }
}
