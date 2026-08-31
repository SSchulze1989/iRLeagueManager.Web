using Blazored.LocalStorage;
using iRLeagueManager.Web;
using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Mirrors the subset of Server-side registrations (Program.cs) needed by the pages/
// components/ViewModels moved into this project so they can render via WebAssembly
// (@rendermode InteractiveAuto). Auth state itself is not re-implemented here: it is
// received from the Server via PersistentAuthenticationStateProvider, which reads the
// UserInfo snapshot persisted server-side by JwtAuthenticationStateProvicer.
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLeagueApiService();
builder.Services.AddLeagueApiClient(config => config
    .UseBaseAddress(builder.Configuration["APIServer"] ?? string.Empty)
    .UseTokenStore<BlazoredLocalStorageTokenStore>());

builder.Services.AddScoped<ClientLocalTimeProvider>();

builder.Services.AddTrackList();
builder.Services.AddViewModels();
builder.Services.AddExporters();
builder.Services.AddMarkdown();
builder.Services.AddLocalization();

builder.Services.AddMudServices(config =>
{
    config.PopoverOptions.Mode = PopoverMode.Legacy;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

var host = builder.Build();

// Hydrate the external API token store with the id token persisted alongside the user's
// identity (see PersistentAuthenticationStateProvider/UserInfo), so ILeagueApiClient calls
// made from WebAssembly-rendered pages are authenticated without waiting for a separate
// round trip. Mirrors Routes.razor's SyncExternalApiTokenAsync (Server-side equivalent).
var authStateProvider = host.Services.GetRequiredService<AuthenticationStateProvider>();
if (authStateProvider is PersistentAuthenticationStateProvider persistentAuthStateProvider
    && string.IsNullOrEmpty(persistentAuthStateProvider.ApiIdToken) == false)
{
    var tokenStore = host.Services.GetRequiredService<iRLeagueApiCore.Client.Http.ITokenStore>();
    await tokenStore.SetIdTokenAsync(persistentAuthStateProvider.ApiIdToken);
}

await host.RunAsync();
