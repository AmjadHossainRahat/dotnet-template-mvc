using Application.Features.Auth.Register;
using Application.Interfaces;
using Application.Mediator;
using FluentValidation;
using Infrastructure.Database;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers application-level services (CQRS handlers, mediator, validators, DTOs).
        /// Implementation should be enriched as features are added.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register mediator abstraction (implementation to be added in Application/Mediator)
            services.AddScoped<IMediator, Application.Mediator.Mediator>();

            // Register validators that were discovered in Startup via AddValidatorsFromAssembly
            // (FluentValidation validators are already registered by AddValidatorsFromAssembly)
            // If you prefer explicit registration:
            //services.AddSingleton<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();

            // Ensure Mapster config is available (Startup registers TypeAdapterConfig.GlobalSettings)
            // and ServiceMapper is registered there.

            return services;
        }

        /// <summary>
        /// Registers the infrastructure services: EF DbContext, repositories, cache, sessions, etc.
        /// This file intentionally contains only the skeleton so the infrastructure step can add real implementations.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // EF Core DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TemplateProjectDb")); // replace with SQL Server/PostgreSQL later

            // Generic repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        /// <summary>
        /// Registers authentication related services (cookie auth, external providers) and session storage options.
        /// </summary>
        public static IServiceCollection AddAuthAndSession(this IServiceCollection services, IConfiguration configuration)
        {
            // Cookie authentication and policies should be configured here.
            // Session store selection (InProc/Distributed Redis/InMemory) will be added later.
            // Example placeholder:
            // services.AddAuthentication(...).AddCookie(...);
            // services.AddSession(...);

            var sessionConfig = configuration.GetSection("SessionOptions");
            var mode = sessionConfig.GetValue<string>("Mode") ?? "InProc";
            var idleTimeout = TimeSpan.FromMinutes(sessionConfig.GetValue<int>("IdleTimeoutMinutes", 30));
            var cookieName = sessionConfig.GetValue<string>("CookieName") ?? ".MyApp.Session";

            // Configure authentication cookie
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = cookieName;
                    options.ExpireTimeSpan = idleTimeout;
                });

            // Configure session storage
            switch (mode.ToLower())
            {
                case "inproc":
                    services.AddDistributedMemoryCache(); // fallback to memory
                    break;

                case "inmemory":
                    services.AddDistributedMemoryCache();
                    break;

                case "redis":
                    var redisConnection = sessionConfig.GetValue<string>("RedisConnection") ?? "localhost:6379";
                    var redisInstance = sessionConfig.GetValue<string>("RedisInstanceName") ?? "MyApp_";

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisConnection;
                        options.InstanceName = redisInstance;
                    });
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported session mode: {mode}");
            }

            // Add session
            services.AddSession(options =>
            {
                options.Cookie.Name = cookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = idleTimeout;
            });

            return services;
        }
    }
}
