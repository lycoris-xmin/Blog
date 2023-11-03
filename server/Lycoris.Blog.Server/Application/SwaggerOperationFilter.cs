using Lycoris.Blog.Server.FilterAttributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lycoris.Blog.Server.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attrs = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).Union(context.MethodInfo.GetCustomAttributes(true));
            var includeApiKey = (attrs?.OfType<AppAuthenticationAttribute>().Any() ?? false) || (attrs?.OfType<WebAuthenticationAttribute>().Any() ?? false);

            if (includeApiKey)
            {
                operation.Security = new List<OpenApiSecurityRequirement>()
                {
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "ApiKey"
                                    }
                                },
                                new List<string>()
                            }
                        }
                };
            }
            else
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
            }
        }
    }
}
