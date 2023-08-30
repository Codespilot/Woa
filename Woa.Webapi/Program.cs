using Serilog;
using Serilog.Events;
using _Host = Microsoft.Extensions.Hosting.Host;

namespace Woa.Webapi;

public class Program
{
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration().Enrich
		                                      .FromLogContext()
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
			CreateHostBuilder(args).Build().Run();
		}
		catch (Exception exception)
		{
			Log.Fatal(exception, "Application terminated unexpectedly");
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}

	private static IHostBuilder CreateHostBuilder(string[] args)
	{
		return _Host.CreateDefaultBuilder(args)
		            .UseSerilog()
		            .ConfigureWebHostDefaults(webBuilder =>
		            {
			            webBuilder.CaptureStartupErrors(true);
			            webBuilder.UseStartup<Startup>();
		            });
	}
}