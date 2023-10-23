using AntDesign.ProLayout;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Woa.Webapp.Models;
using Woa.Webapp.Rest;

namespace Woa.Webapp;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddAntDesign();
		builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("LayoutSettings"));

		builder.Services.AddOptions();
		builder.Services.AddAuthorizationCore();
		builder.Services
		       .AddScoped<IdentityAuthenticationStateProvider>()
		       .AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<IdentityAuthenticationStateProvider>())
		       .AddBlazoredLocalStorageAsSingleton()
		       .AddObjectMapping()
		       .AddObjectValidation()
		       .AddHttpClientApi(options =>
		       {
			       var baseUrl = builder.Configuration.GetValue<string>("Api:BaseUrl");
			       var timeout = builder.Configuration.GetValue<int>("Api:Timeout");
			       if (string.IsNullOrEmpty(baseUrl))
			       {
				       baseUrl = builder.HostEnvironment.BaseAddress;
			       }

			       options.BaseUrl = baseUrl;
			       options.Timeout = TimeSpan.FromMilliseconds(timeout);
		       });

		await builder.Build().RunAsync();
	}
}