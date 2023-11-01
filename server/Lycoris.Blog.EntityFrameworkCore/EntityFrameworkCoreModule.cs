using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common;
using Lycoris.Blog.EntityFrameworkCore.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Blog.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityFrameworkCoreModule : LycorisRegisterModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public override void SerivceRegister(IServiceCollection services)
        {
            services.AddScoped(sp => sp.GetRequiredService<MySqlContextScopedFactory>().CreateDbContext());

            services.AddPooledDbContextFactory<MySqlContext>(opt =>
            {
                opt.UseMySql(AppSettings.Sql.ConnectionString, new MySqlServerVersion(ServerVersion.Parse(AppSettings.Sql.Version)), builder =>
                {
                    builder.MinBatchSize(4);
                    builder.CommandTimeout(60);
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    builder.UseNewtonsoftJson();
                });

                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                if (AppSettings.IsDebugger)
                {
                    //opt.LogTo(x =>
                    //{
                    //    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    //    Console.WriteLine(x);
                    //    Console.ResetColor();
                    //}, LogLevel.Information);

                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }

                // 读写分离 （还未测试）
                // opt.AddInterceptors(new MasterSlaveCommandInterceptor());
            });
        }
    }
}