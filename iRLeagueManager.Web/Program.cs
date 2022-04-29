using iRLeagueApiCore.Client;
using iRleagueManager.Web.ViewModels;
using iRLeagueManager.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
var apiHttpClient = new HttpClient();
apiHttpClient.BaseAddress = new Uri("https://irleaguemanager.net/irleagueapi/");
builder.Services.AddSingleton<ILeagueApiClient>(sp => new LeagueApiClient(sp.GetRequiredService<ILogger<LeagueApiClient>>(), apiHttpClient));
builder.Services.AddMvvm();
builder.Services.AddScoped<LeaguesViewModel>();

await builder.Build().RunAsync();
