using Blazored.LocalStorage;
using iRLeagueManager.Web;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using MudBlazor.Services;
using MudBlazor;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

// Add services to the container.
builder.Services.AddScoped<ServerAuthenticationStateProvider>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName, config =>
{
    config.DefaultRequestHeaders.UserAgent.ParseAdd($"iRLeagueManager/{Assembly.GetEntryAssembly()!.GetName().Version!.Major}.{Assembly.GetEntryAssembly()!.GetName().Version!.Minor}.{Assembly.GetEntryAssembly()!.GetName().Version!.Build} (iRLeaguemanager Web App)");
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMvvm();
builder.Services.AddLeagueApiService();

builder.Services.AddLeagueApiClient(config => config
    .UseBaseAddress(builder.Configuration["APIServer"] ?? string.Empty)
    .UseTokenStore<BrowserProtectedStorageTokenStore>());

builder.Services.AddScoped<ClientLocalTimeProvider>();
builder.Services.AddScoped<SharedStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvicer>();
builder.Services.AddTrackList();
builder.Services.AddViewModels();
builder.Services.AddSingleton<IAuthorizationHandler, ProfileHandler>();
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy(ProfileOwnerRequirement.Policy, policy => policy.AddRequirements(new ProfileOwnerRequirement()));
});

builder.Services.AddLocalization();
builder.Services.AddMarkdown();

builder.Services.AddMudServices(config =>
{
    config.PopoverOptions.Mode = PopoverMode.Legacy;
});

var app = builder.Build();

app.UsePathBase("/");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "en-US", "de" })
    .AddSupportedUICultures(new[] { "en-US", "de" }));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
