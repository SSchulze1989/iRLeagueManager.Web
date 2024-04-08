using AspNetCoreRateLimit;
using Aydsko.iRacingData;
using iRLeagueApiCore.Common.Converters;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Extensions;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace iRLeagueApiCore.Server;

public sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<IISServerOptions>(options =>
            options.AutomaticAuthentication = false);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("*") // this is only valid for dev environment
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddControllers().AddJsonOptions(option =>
        {
            option.JsonSerializerOptions.Converters.Add(new JsonTimeSpanConverter());
        });
        services.AddSwaggerGen(c =>
        {
            var readHostUrls = Configuration["ASPNETCORE_HOSTURLS"];
            if (readHostUrls != null)
            {
                var hostUrls = readHostUrls.Split(';');
                foreach (var hostUrl in hostUrls)
                {
                    c.AddServer(new OpenApiServer() { Url = hostUrl });
                }
            }
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "iRLeagueApiCore.Server", Version = "v1" });
            c.OperationFilter<OpenApiParameterIgnoreFilter>();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                  "Enter 'Bearer' [space] and then your token in the text input below." +
                  "\r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
            });
            var xmlPath = Path.Combine(AppContext.BaseDirectory, "iRLeagueApiCore.Server.xml");
            c.IncludeXmlComments(xmlPath);
            xmlPath = Path.Combine(AppContext.BaseDirectory, "iRLeagueApiCore.Common.xml");
            c.IncludeXmlComments(xmlPath);
            c.MapType<TimeSpan>(() => new OpenApiSchema()
            {
                Type = "string",
                Format = "duration",
                Pattern = "hh:mm:ss.fffff",
                Example = new OpenApiString(new TimeSpan(0, 1, 2, 34, 567).ToString(@"hh\:mm\:ss\.fffff"))
            });
            c.MapType<TimeSpan?>(() => new OpenApiSchema()
            {
                Type = "string",
                Format = "duration",
                Nullable = true,
                Pattern = "hh:mm:ss.fffff",
                Example = new OpenApiString(new TimeSpan(0, 1, 2, 34, 567).ToString(@"hh\:mm\:ss\.fffff"))
            });
        });

        // try get connection string
        services.AddDbContextFactory<UserDbContext, UserDbContextFactory>();
        services.AddScoped(x =>
            x.GetRequiredService<IDbContextFactory<UserDbContext>>().CreateDbContext());
        services.AddSingleton<LeagueDbContextFactory>();
        services.AddScoped<RequestLeagueProvider>();
        services.AddScoped<ILeagueProvider>(x => x.GetRequiredService<RequestLeagueProvider>());
        services.AddScoped(x =>
            x.GetRequiredService<LeagueDbContextFactory>().CreateDbContext(x.GetRequiredService<ILeagueProvider>()));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        // Adding Jwt Bearer  
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = Configuration["JWT:ValidAudience"],
                ValidIssuer = Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 1;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        });

        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddInMemoryRateLimiting();

        services.AddTrackImporter();

        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddEmailService();
        services.AddResultService();
        services.AddBackgroundQueue();

        services.AddSingleton<ICredentials, CredentialList>(x => new(Configuration.GetSection("Credentials")));

        services.AddIRacingDataApi(options =>
        {
            options.UserAgentProductName = "iRLeagueApiCore";
            options.UserAgentProductVersion = Assembly.GetEntryAssembly()!.GetName().Version!;
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseCors();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            //c.SwaggerEndpoint("./v1/swagger.json", "TestDeploy v1");
            //c.RoutePrefix = string.Empty;
            string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "./swagger/" : "";
            c.SwaggerEndpoint($"{swaggerJsonBasePath}v1/swagger.json", "iRLeagueApiCore.Server v1");
        });

        //app.UseHttpsRedirection();

        app.UseFileServer();

        app.UseRouting();

        app.UseIpRateLimiting();

        app.UseSerilogRequestLogging(options =>
        {
            // Customize the message template
            options.MessageTemplate = "{RemoteIpAddress:l} {RequestScheme:l} {RequestMethod:l} {RequestPath:l} responded {StatusCode} in {Elapsed:0.0000} ms {RequestReferer} {RequestAgent} {UserName}";

            // Emit debug-level events instead of the defaults
            options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;

            // Attach additional properties to the request completion event
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestAgent", httpContext.Request.Headers["User-Agent"].ToString());
                diagnosticContext.Set("RequestReferer", httpContext.Request.GetTypedHeaders().Referer?.ToString() ?? string.Empty);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? string.Empty);
                diagnosticContext.Set("UserId", httpContext.User.GetUserId());
            };
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
