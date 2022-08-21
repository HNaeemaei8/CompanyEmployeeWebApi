using AspNetCoreRateLimit;
using Contracts.IServices;
using Contracts.IServices.LoggerService;
using Entities;
using Entities.Context;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApi.Infrastructure.Extentions;

namespace MyApi.Infrastructure.Extentions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection service)

               =>
               service.AddCors(options =>
               {
                   options.AddPolicy("CorsPolicy", builder =>
                   builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());


               });


        public static void ConfigureIISIntegration(this IServiceCollection services)
            =>
            services.Configure<IISOptions>(options =>
            { });


        public static void ConfigureLoggerService(this IServiceCollection
services) => services.AddScoped<ILoggerManager, LoggerManager>();

        public static void SqlContext(this IServiceCollection services, IConfiguration configuration)
            =>
            services.AddDbContext<CompanyEmployeeDbContext>(option => option.UseSqlServer(configuration.GetConnectionString("SqlConnection"), b => b.MigrationsAssembly("WebApi")));


        public static void ConfigureRepositoryManager(this IServiceCollection services)
            =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();


        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
  =>
  builder.AddMvcOptions(config => config.OutputFormatters.Add(new
  CsvOutputFormatter()));



        public static void ConfigureVersioning(this IServiceCollection services)
            => services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");

            });


        public static void ConfigureResponseCaching(this IServiceCollection services)
            => services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
            => services.AddHttpCacheHeaders(
                (expirationOpt) =>
                {

                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;

                },
                (expirationOpt) =>
                {
                    expirationOpt.MustRevalidate = true;
                }
                );


        public static void ConfigureRateLimitingOptions(this IServiceCollection
        services)
        {
            var rateLimitRules = new List<RateLimitRule>
             {
             new RateLimitRule
             {
             Endpoint = "*",
             Limit= 30,
             Period = "5m"
             }
             };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore,
            MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddSingleton<IRateLimitConfiguration,
            RateLimitConfiguration>();



        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(o =>
             {
                 o.Password.RequireDigit = true;
                 o.Password.RequireLowercase = true;
                 o.Password.RequireUppercase = true;
                 o.Password.RequireNonAlphanumeric = false;
                 o.Password.RequiredLength = 10;
                 o.User.RequireUniqueEmail = true;


             });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole),
            builder.Services);
            builder.AddEntityFrameworkStores<CompanyEmployeeDbContext>()
            .AddDefaultTokenProviders();

        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {

            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                var secretKey = Environment.GetEnvironmentVariable("SECRET");
                services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                        ValidAudience =
                        jwtSettings.GetSection("validAudience").Value,
                        IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });
            }

        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {


                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CompanyEmployee.API",
                    Version = "v1",




        Description = "CompanyEmployees API by Zahra Bayat",
                    Contact = new OpenApiContact
                    {
                        Name = "Zahra Bayat",
                        Email = "BytZahra@gmail.com",
                        Url = new
              Uri("https://www.linkedin.com/in/zahrabayat"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CompanyEmployees API ",
                    }






                }); ;
                s.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "CompanyEmployee.API",
                    Version = "v2"
                });



                var xmlFile =$"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);





                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
            {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Name = "Bearer",
            },
            new List<string>()
            }
            });
            });


        }
    }
}
    

    
