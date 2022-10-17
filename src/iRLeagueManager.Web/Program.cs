using Blazored.LocalStorage;
using Blazored.Modal;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Common.Converters;
using iRLeagueManager.Web;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Server.Data;
using iRLeagueManager.Web.Shared;
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
builder.Services.AddScoped<ServerAuthenticationStateProvider>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMvvm();
builder.Services.AddLeagueApiService();

var apiHttpClient = new HttpClient();
apiHttpClient.BaseAddress = new Uri(builder.Configuration["APIServer"]);
builder.Services.AddScoped<JsonSerializerOptions>(configure =>
{
    var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    jsonOptions.Converters.Add(new JsonStringEnumConverter());
    jsonOptions.Converters.Add(new JsonTimeSpanConverter());
    jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    return jsonOptions;
});
builder.Services.AddScoped<LeagueApiClientFactory>();
builder.Services.AddScoped<ITokenStore, AsyncTokenStore>();
builder.Services.AddScoped<IAsyncTokenProvider>(x => x.GetRequiredService<ITokenStore>());
builder.Services.AddScoped(sp => sp.GetRequiredService<LeagueApiClientFactory>().CreateClient());
builder.Services.AddScoped<SharedStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvicer>();
builder.Services.AddViewModels();

builder.Services.AddBlazoredModal();

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
