using Blazored.LocalStorage;
using iRLeagueManager.Web;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Endpoints;
using iRLeagueManager.Web.Middleware;
using iRLeagueManager.Web.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using MudBlazor;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

// Add services to the container.
builder.Services.AddScoped<ServerAuthenticationStateProvider>();
//builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName, config =>
{
    config.DefaultRequestHeaders.UserAgent.ParseAdd($"iRLeagueManager/{Assembly.GetEntryAssembly()!.GetName().Version!.Major}.{Assembly.GetEntryAssembly()!.GetName().Version!.Minor}.{Assembly.GetEntryAssembly()!.GetName().Version!.Build} (iRLeaguemanager Web App)");
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLeagueApiService();

builder.Services.AddLeagueApiClient(config => config
    .UseBaseAddress(builder.Configuration["APIServer"] ?? string.Empty)
    .UseTokenStore<BrowserProtectedStorageTokenStore>());

builder.Services.AddScoped<ClientLocalTimeProvider>();
builder.Services.AddScoped<SharedStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvicer>();
builder.Services.AddScoped<IAuthenticationService, iRLeagueManager.Web.Data.AuthenticationService>();

// Cookie-based JWT authentication (Phase 1): the JWT stored in the HttpOnly
// "X-Access-Token" cookie is translated into a bearer token by JwtCookieMiddleware
// and validated here using the same signing configuration used to create it.
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("Jwt:Secret configuration value is missing.");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });
builder.Services.AddTrackList();
builder.Services.AddViewModels();
builder.Services.AddExporters();
builder.Services.AddSingleton<IAuthorizationHandler, ProfileHandler>();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ProfileOwnerRequirement.Policy, policy => policy.AddRequirements(new ProfileOwnerRequirement()));
builder.Services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
builder.Services.AddLocalization();
builder.Services.AddMarkdown();
builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

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

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseJwtCookieMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapAuthEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(["en-US", "de"])
    .AddSupportedUICultures(["en-US", "de"]));

app.Run();
