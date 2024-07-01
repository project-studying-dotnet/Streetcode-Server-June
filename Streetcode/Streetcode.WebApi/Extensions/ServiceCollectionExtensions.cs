using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using StackExchange.Redis;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Services.Logging;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Services.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Services.BlobStorageService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Services.Instagram;
using Streetcode.BLL.Interfaces.Payment;
using Streetcode.BLL.Services.Payment;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Services.Text;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.Tokens;

namespace Streetcode.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentityService(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<StreetcodeDbContext>()
            .AddDefaultTokenProviders();
    }
    
    public static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }

    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddRepositoryServices();
        services.AddFeatureManagement();
        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(currentAssemblies);
        services.AddValidatorsFromAssemblies(currentAssemblies);
        services.AddMediatR(currentAssemblies);
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInstagramService, InstagramService>();
        services.AddScoped<ITextService, AddTermsToTextService>(); 
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    public static void AddCachingService(this IServiceCollection services, ConfigurationManager configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        var redisConnectionString = configuration.GetSection(environment).GetConnectionString("ReddisConnection");
        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
    }

    public static void AddPipelineBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachibleQueryBehavior<,>));
    }
    
    public static void AddTokensConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["TokensConfiguration:SecretKey"] !));

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = configuration["TokensConfiguration:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["TokensConfiguration:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = key
        };

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
            });
    }
    
    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
        var connectionString = configuration.GetSection(environment).GetConnectionString("DefaultConnection")
                              ?? configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException($"'{environment}.DefaultConnection' not found!");

        var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        if(emailConfig != null)
        {
            services.AddSingleton(emailConfig);
        }
        
        var tokensConfig = configuration.GetSection("TokensConfiguration").Get<TokensConfiguration>();
        if(tokensConfig != null)
        {
            services.AddSingleton(tokensConfig);
        }
        
        services.AddDbContext<StreetcodeDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(typeof(StreetcodeDbContext).Assembly.GetName().Name);
                opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
            });
        });

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer();

        var corsConfig = configuration.GetSection("CORS").Get<CorsConfiguration>();
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddHsts(opt =>
        {
            opt.Preload = true;
            opt.IncludeSubDomains = true;
            opt.MaxAge = TimeSpan.FromDays(30);
        });

        services.AddLogging();
        services.AddControllers();
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
            opt.CustomSchemaIds(x => x.FullName);
            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    public class CorsConfiguration
    {
        public List<string> AllowedOrigins { get; set; }
        public List<string> AllowedHeaders { get; set; }
        public List<string> AllowedMethods { get; set; }
        public int PreflightMaxAge { get; set; }
    }
}
