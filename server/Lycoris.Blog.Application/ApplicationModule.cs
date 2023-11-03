using Lycoris.Autofac.Extensions;
using Lycoris.Autofac.Extensions.Impl;
using Lycoris.Blog.Application.Schedule.Jobs;
using Lycoris.Blog.Core.Interceptors.Transactional;
using Lycoris.Quartz.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Lycoris.Blog.Application
{
    public class ApplicationModule : LycorisRegisterModule
    {
        public override void ModuleRegister(LycorisModuleBuilder builder)
        {
            // 为当前程序集注册一个AOP拦截器
            builder.InterceptedBy<TransactionalInterceptor>();
        }

        public override void SerivceRegister(IServiceCollection services)
        {
            var minio = new MinioClient();
            services.AddSingleton(minio);

            // 任务调度
            services.AddQuartzSchedulerCenter(opt =>
            {
                opt.AddJob<ScheduleQueueJob>();
                opt.AddJob<DBRegularCleanerJob>();
                opt.AddJob<WebStatisticsJob>();
                opt.AddJob<ServerMonitorJob>();
                opt.AddJob<StaticFileCleanerJob>();
                opt.DisabledRunHostedJob();
            });
        }
    }
}