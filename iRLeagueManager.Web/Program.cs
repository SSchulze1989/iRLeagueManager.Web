using Blazored.LocalStorage;
using iRLeagueApiCore.Client.Http;
using iRLeagueManager.Web;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Server.Data;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<ServerAuthenticationStateProvider>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMvvm();
builder.Services.AddLeagueApiService();

var apiHttpClient = new HttpClient();
apiHttpClient.BaseAddress = new Uri(builder.Configuration["APIServer"]);
builder.Services.AddScoped<LeagueApiClientFactory>();
builder.Services.AddScoped<ITokenStore, AsyncTokenStore>();
builder.Services.AddScoped(sp => sp.GetRequiredService<LeagueApiClientFactory>().CreateClient());
builder.Services.AddScoped<SharedStateService>();
builder.Services.AddViewModels();

var app = builder.Build();

app.UsePathBase("/app");

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
