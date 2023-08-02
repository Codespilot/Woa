using Refit;
using Serilog;
using Serilog.Events;
using Woa.Webapi;
using Woa.Webapi.Apis;
using Woa.Webapi.Services;

Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Debug)
                                             .WriteTo.File("Logs/Debug/logs.log", rollingInterval: RollingInterval.Day))
             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Warning)
                                             .WriteTo.File("Logs/Warning/logs.log", rollingInterval: RollingInterval.Day))
             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Error)
                                             .WriteTo.File("Logs/Error/logs.log", rollingInterval: RollingInterval.Day))
             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Information)
                                             .WriteTo.File("Logs/Info/logs.log", rollingInterval: RollingInterval.Day))
             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Fatal)
                                             .WriteTo.File("Logs/Fatal/logs.log", rollingInterval: RollingInterval.Day))
             .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
// Add services to the container.
//HttpClient.DefaultProxy = new WebProxy("localhost", 8888);
    builder.Services.AddControllers();
    builder.Services.AddMemoryCache();
    builder.Services.AddSupabaseClient(builder.Configuration);
    builder.Services.AddRefitClient<IWechatApi>(new RefitSettings { Buffered = true, ContentSerializer = new NewtonsoftJsonContentSerializer() })
           .ConfigureHttpClient((_, client) =>
           {
               client.BaseAddress = new Uri("https://api.weixin.qq.com");
           });
    builder.Services.AddWechatMessageHandler();

    builder.Services.AddHostedService<ChatbotBackgroundService>();
    builder.Services.AddRecurringJobService();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}