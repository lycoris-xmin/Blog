using Lycoris.Blog.Core.Logging;
using Lycoris.Blog.Model.Cnstants;
using Lycoris.Blog.Model.Exceptions;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Lycoris.Blog.Server.FilterAttributes
{
    /// <summary>
    /// 接口全局异常捕获
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = false)]
    public class ApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public ApiExceptionHandlerAttribute(ILycorisLoggerFactory factory) => _logger = factory.CreateLogger<ApiExceptionHandlerAttribute>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            if (context.Exception is FriendlyException friendlyException)
                FriendlyExceptionHandler(context, friendlyException);
            else if (context.Exception is HttpStatusException httpStatusException)
                HttpStatusExceptionHanlder(context, httpStatusException);
            else if (context.Exception is OutputException outputException)
                OutputExceptionHanlder(context, outputException);
            else
                OtherExceptionHanlder(context, context.Exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            if (context.Exception is FriendlyException friendlyException)
                FriendlyExceptionHandler(context, friendlyException);
            else if (context.Exception is HttpStatusException httpStatusException)
                HttpStatusExceptionHanlder(context, httpStatusException);
            else if (context.Exception is OutputException outputException)
                OutputExceptionHanlder(context, outputException);
            else
                OtherExceptionHanlder(context, context.Exception);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void FriendlyExceptionHandler(ExceptionContext context, FriendlyException ex)
        {
            _logger.Warn($"handler friendly exception: {ex.LogMessage}");

            var res = new BaseOutput()
            {
                ResCode = ResCodeEnum.Friendly,
                ResMsg = ex.Message,
                TraceId = context.HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            context.Result = new JsonResult(res) { ContentType = "application/json", StatusCode = 200 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void HttpStatusExceptionHanlder(ExceptionContext context, HttpStatusException ex)
        {
            _logger.Warn($"handler httpstatus exception: {ex.Message}", ex);

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, "");

            context.Result = new ContentResult { Content = "", StatusCode = (int)ex.HttpStatusCode };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void OutputExceptionHanlder(ExceptionContext context, OutputException ex)
        {
            if (ex.ResCode == ResCodeEnum.TokenExpired)
                _logger.Warn($"handler output exception: {ex.Message}");
            else if (ex.ResCode == ResCodeEnum.Friendly)
                _logger.Warn($"handler output exception: {(ex.ResMsg.IsNullOrEmpty() ? ex.Message : ex.ResMsg)}");
            else if (ex.ResCode == ResCodeEnum.Success && !ex.ResMsg.IsNullOrEmpty())
                _logger.Warn($"handler output exception: succes -> {ex.ResMsg}");
            else
                _logger.Warn($"handler output exception: {ex.Message}", ex);

            var res = new BaseOutput()
            {
                ResCode = ex.ResCode,
                ResMsg = ex.ResMsg,
                TraceId = context.HttpContext.Items.GetValue(HttpItems.TraceId)
            };

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, res.ToJson());

            context.Result = new JsonResult(res) { ContentType = "application/json", StatusCode = 200 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        private void OtherExceptionHanlder(ExceptionContext context, Exception ex)
        {
            _logger.Error($"hanlder exception:{ex.GetType().FullName}", ex);

            context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, ex.Message);

            context.HttpContext.Items.AddOrUpdate(HttpItems.Exception, ex.Message);
            context.HttpContext.Items.AddOrUpdate(HttpItems.StackTrace, ex.StackTrace!);

            context.Result = new ContentResult { Content = "", StatusCode = 500 };
        }
    }
}
