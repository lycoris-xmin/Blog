using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Blog.Application;
using Lycoris.Blog.Application.SignalR.Hubs;
using Lycoris.Blog.Common;
using Lycoris.Blog.Core;
using Lycoris.Blog.EntityFrameworkCore;
using Lycoris.Blog.Model;
using Lycoris.Blog.Model.Cnstants;
using Lycoris.Blog.Server.Application;
using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Blog.Server.Application.Swaggers;
using Lycoris.Blog.Server.FilterAttributes;
using Lycoris.Blog.Server.Middlewares;
using Lycoris.Common.ConfigurationManager;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using System.IO.Compression;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// �����ļ�
SettingManager.JsonConfigurationInitialization(AppSettings.Path.JsonFile);

// 
builder.Host.ConfigureHostConfiguration(configuration => configuration.AddCommandLine(args));

// ��־���
builder.Host.UseSerilog((context, config) =>
{
    AppSettings.Serilog.SerilogOverrideOptions.ForEach(OverrideOption => config.MinimumLevel.Override(OverrideOption.Source, OverrideOption.MinLevel.ToEnum<LogEventLevel>()));

    config.MinimumLevel.Is(AppSettings.Serilog.MinLevel.ToEnum<LogEventLevel>());

    if (AppSettings.Serilog.Console)
        config.WriteTo.Console();

    if (AppSettings.IsDebugger)
        config.WriteTo.Debug();

    if (AppSettings.Serilog.File)
    {
        var logPath = AppSettings.IsDebugger
        ? Path.Combine(AppSettings.Path.RootPath, "AppData/logs", "log.txt")
        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData/logs", "log.txt");

        var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - {Level:u3} - {SourceContext:l} - {Message:lj}{NewLine}{Exception}";
        config.WriteTo.File(logPath, outputTemplate: template, rollingInterval: RollingInterval.Day, shared: true, fileSizeLimitBytes: 10 * 1025 * 1024, rollOnFileSizeLimit: true);
    }
});

// �滻ϵͳ�Դ���DI����ΪAutofac
builder.UseAutofacExtensions(builder =>
{
    // ģ��ע��
    builder.AddLycorisRegisterModule<ModelModule>();
    // ģ��ע��
    builder.AddLycorisRegisterModule<CommonModule>();
    // ģ��ע��
    builder.AddLycorisRegisterModule<EntityFrameworkCoreModule>();
    // ģ��ע��
    builder.AddLycorisRegisterModule<CoreModule>();
    // ģ��ע��
    builder.AddLycorisRegisterModule<ApplicationModule>();
    // 
    builder.EnabledLycorisMultipleService = true;
});

// �������ж˿ں�
builder.WebHost.UseUrls($"http://*:{AppSettings.Application.HttpPort}");

// AutoMapperע��
builder.Services.AddAutoMapperService(opt => opt.AddMapperProfile<ViewModelMapperProfile>().AddMapperProfile<ApplicationMapperProfile>());

// ע�����������Ľ���
builder.Services.AddHttpContextAccessor();

// ���������ƶ���Դ�Ŀ�������
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        if (AppSettings.Application.Cors.Origins.HasValue())
            builder.WithOrigins(AppSettings.Application.Cors.Origins);
        else
            builder.AllowAnyOrigin();

        if (AppSettings.Application.Cors.Methods.HasValue())
            builder.WithMethods(AppSettings.Application.Cors.Methods);
        else
            builder.AllowAnyMethod();

        if (AppSettings.Application.Cors.Headers.HasValue())
            builder.WithHeaders(AppSettings.Application.Cors.Headers);
        else
            builder.AllowAnyHeader();

        builder.AllowCredentials();
    });
});

// ������ע��
builder.Services.AddControllers(opt =>
{
    // �ӿ�ȫ���쳣��׽
    opt.Filters.Add<ApiExceptionHandlerAttribute>();

    // XSS����
    opt.Filters.Add<GanssXssFilterAttribute>();

    // ����������
    opt.Filters.Add<RequestContextAttribute>();
})
.AddNewtonsoftJson(opt =>
{
    // �������շ崦��
    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    // ����ʱ���ʽ
    opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    // ����ѭ������
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    // ���Կ�ֵ����
    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

builder.Services.AddSignalR().AddNewtonsoftJsonProtocol(opt =>
{
    // �������շ崦��
    opt.PayloadSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    // ����ʱ���ʽ
    opt.PayloadSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    // ����ѭ������
    opt.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    // ���Կ�ֵ����
    opt.PayloadSerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

// ������֤ʧ�ܴ���
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // ��ȡ��֤ʧ�ܵ�ģ���ֶ� 
        var errors = context.ModelState.Where(e => e.Value != null && e.Value.Errors.Count > 0).Select(e => e.Value?.Errors?.FirstOrDefault()?.ErrorMessage).ToList();
        context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, string.Join(",", errors));
        return new BadRequestResult();
    };
});

// ���ñ�������ֵ
builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 60000000);

// ���ÿ���ͬ�������ȡ������
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);

// ��Ӧѹ������
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    // ��չһЩ���� (MimeTypes����һЩ����������,���Դ�ϵ㿴��)
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/html; charset=utf-8", "application/xhtml+xml", "application/atom+xml", "image/svg+xml" });
});

// �������� OpenApi
if (AppSettings.IsDebugger)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.OperationFilter<SwaggerOperationFilter>();
        opt.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
        {
            Name = HttpHeaders.Authentication,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Description = "��������"
        });

        // �˴���չʾ���ݰ汾����ķ�����ʵ�����������Ŀ���иĶ�
        opt.SwaggerDoc(ApiVersionGroup.V1, new OpenApiInfo() { Title = "Lycoris.Blog", Version = ApiVersionGroup.V1 });
        // ����ж���汾�⣬����Ҫ�ֱ�����ÿ���汾����Ϣ
        //opt.SwaggerDoc(ApiVersionGroup.V2, new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Lycoris.Blog", Version = ApiVersionGroup.V2 });

        // ע���ĵ�
        var source = Assembly.GetEntryAssembly()?.GetName().Name?.Replace(".Server", "");
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{source}.Server.xml"));
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{source}.Model.xml"));
    });
}

// ��ӳ�����������
builder.Services.AddHostedService<ApplicationHostedService>();

var app = builder.Build();

// ʹ��AutoMapperȫ����չ
app.UseAutoMapperExtensions();

// �������� OpenApi
if (AppSettings.IsDebugger)
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        // �˴���չʾ���ݰ汾����ķ�����ʵ�����������Ŀ���иĶ�
        x.SwaggerEndpoint($"/swagger/{ApiVersionGroup.V1}/swagger.json", $"Lycoris.Blog - {ApiVersionGroup.V1}");
        // ����ж���汾�⣬����Ҫ�ֱ�����ÿ���汾����Ϣ
        //x.SwaggerEndpoint($"/swagger/{ApiVersionGroup.V2}/swagger.json", $"Lycoris.Blog - {ApiVersionGroup.V2}");
    });
}

// �м���쳣��׽
app.UseMiddleware<ExceptionHandlerMiddleware>();

// ��־��¼�м��
app.UseMiddleware<HttpLoggingMiddleware>();

// ��Ӧѹ��
app.UseResponseCompression();

// ·��
app.UseRouting();

// �����м��
app.UseCors();

// Cookie����
app.UseCookiePolicy(new CookiePolicyOptions() { HttpOnly = HttpOnlyPolicy.Always });

// �ս������
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<HomeHub>("Api/Lycoris/Hub/Home");
    endpoints.MapHub<ChatHub>("Api/Lycoris/Hub/Chat");
    endpoints.MapHub<DashboardHub>("Api/Lycoris/Hub/Dashboard");
});

// ��������
app.Run();

