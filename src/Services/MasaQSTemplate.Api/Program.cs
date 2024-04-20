using MasaQSTemplate.Api.Middlewares;
using MasaQSTemplate.BaseModule;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SqlSugar;

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .Enrich.FromLogContext()
    .WriteTo.File($"{AppContext.BaseDirectory}logs/logs-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services
        .AddMapster()
        .AddAutoInject(typeof(AuthDbContext).Assembly, typeof(LogDbContext).Assembly)
        .AddEventBus(eventBusBuilder =>
        {
            //eventBusBuilder.UseMiddleware(typeof(EventLogMiddleware<>));
        })
        .AddMasaIdentity()
        .AddJwt(options =>
        {
            options.Issuer = "MasaQSTemplate";
            options.SecurityKey = builder.Configuration["JwtBearerOptions:IssuerSigningKey"];
        })
        .AddSingleton<IAuthorizationHandler, PermissionAuthorizeHandler>();

    builder.Services
        .AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Audience = "MasaQSTemplate";
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtBearerOptions:IssuerSigningKey"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    builder.Services.AddControllers();

    builder.Services
         // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
         .AddEndpointsApiExplorer()
         .AddSwaggerGen(options =>
         {
             options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
             {
                 Name = "Authorization",
                 Type = SecuritySchemeType.ApiKey,
                 Scheme = "Bearer",
                 BearerFormat = "JWT",
                 In = ParameterLocation.Header,
                 Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xxxxxxxxxxxxxxx\"",
             });
             options.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
             });
         });

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        #region MigrationDb
        foreach (var service in builder.Services.Where(x => x.ServiceType.IsSubclassOf(typeof(SqlSugarDbContext))))
        {
            using var db = (SqlSugarDbContext)app.Services.CreateScope().ServiceProvider.GetService(service.ServiceType);
            {
                db.Init();
            }
        }
        #endregion
    }

    app.UseMiddleware<RequestLogMiddleware>();

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}