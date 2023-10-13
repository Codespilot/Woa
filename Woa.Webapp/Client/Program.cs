using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Woa.Webapp;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
		builder.Services.AddAntDesign();
		builder.Services.Configure<ProSettings>(settings =>
		{
			settings.FixedHeader = true;
			settings.FixSiderbar = true;
			settings.FooterRender = false;
			settings.ContentWidth = "Fluid";
			settings.Layout = Layout.Mix.Name;
			settings.PrimaryColor = "#F5222D";
		});

		await builder.Build().RunAsync();
	}
}
