using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common.Snowflakes;
using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Lycoris.Blog.EntityFrameworkCore.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class PropertyAutoProvider : IPropertyAutoProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public ISnowflakesMaker SnowflakesMaker { get; }

        /// <summary>
        /// 请求上下文
        /// </summary>
        public RequestContext RequestContext { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SnowflakesMaker"></param>
        /// <param name="RequestContext"></param>
        public PropertyAutoProvider(ISnowflakesMaker SnowflakesMaker, RequestContext RequestContext)
        {
            this.SnowflakesMaker = SnowflakesMaker;
            this.RequestContext = RequestContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void InsertIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if ((item.Metadata.ClrType == typeof(long) || item.Metadata.ClrType == typeof(long?)) && item.Metadata.PropertyInfo != null && item.Metadata.PropertyInfo!.GetCustomAttribute<SnowflakeAttribute>(false) != null)
                {
                    if (item.CurrentValue != null && (long)item.CurrentValue > 0)
                        continue;

                    item.CurrentValue = this.SnowflakesMaker.GetSnowflakesId();
                }
                else if (item.Metadata.ClrType == typeof(DateTime))
                {
                    if (item.Metadata.Name.Equals("CreateTime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                            item.CurrentValue = DateTime.Now;
                    }
                }
                else if (this.RequestContext.User?.Id > 0)
                {
                    if (item.Metadata.Name.Equals("CreateUserId", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(long))
                    {
                        if (item.CurrentValue == null || (long)item.CurrentValue == 0)
                            item.CurrentValue = this.RequestContext.User?.Id ?? 0;
                    }

                    if (item.Metadata.Name.Equals("CreateNickName", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.RequestContext.User?.NickName ?? "";
                    }

                    if (item.Metadata.Name.Equals("CreateAvatar", StringComparison.CurrentCultureIgnoreCase) && item.Metadata.ClrType == typeof(string))
                    {
                        if (item.CurrentValue == null || ((string)item.CurrentValue).IsNullOrEmpty())
                            item.CurrentValue = this.RequestContext.User?.Avatar ?? "";
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void UpdateIntercept(EntityEntry entities)
        {
            foreach (var item in entities.Properties)
            {
                if (item.Metadata.ClrType == typeof(DateTime))
                {
                    if (item.Metadata.Name.Equals("UpdateTime", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.CurrentValue == null || (DateTime)item.CurrentValue == DateTime.MinValue)
                            item.CurrentValue = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void DeleteIntercept(EntityEntry entities)
        {

        }
    }
}
