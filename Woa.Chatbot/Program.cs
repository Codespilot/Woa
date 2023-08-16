using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Woa.Chatbot;
using Woa.Chatbot.Services;
using Woa.Common;
using Woa.Sdk;
using Woa.Sdk.OpenAi;

//HttpClient.DefaultProxy = new WebProxy("localhost", 8888);

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
    var host = Host.CreateDefaultBuilder(args)
                   .UseSerilog()
                   .ConfigureServices((context,services) =>
                   {
                       services.AddSupabaseClient()
                               .AddOpenAiApi(context.Configuration.GetSection("Bot:OpenAi"))
                               .AddClaudeApi(context.Configuration.GetSection("Bot:Claude"));
                       services.AddTransient<ChatGptCompletionService>()
                               .AddTransient<ClaudeCompletionService>();
                       services.AddHostedService<ChatCompletionBackgroundService>();
                   })
                   .Build();
    host.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}