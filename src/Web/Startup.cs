using Application;
using Application.Features.Auth.Register;
using Application.Interfaces;
using Application.Mapping;
using Application.Mediator;
using FluentValidation;
using Infrastructure.Authentication;
using Infrastructure.Repository;
using Mapster;
using MapsterMapper;
using Web.Extensions;

namespace Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        // Called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // MVC (no Razor runtime compilation)
            services.AddControllersWithViews();

            // Register FluentValidation validators from Application assembly (core FluentValidation)
            // Note: This registers validators in DI. Integrating them into ASP.NET MVC validation pipeline
            // typically uses FluentValidation.AspNetCore; if you later want automatic model validation,
            // we can add the adapter in a later step. For now, validators are available for manual invocation
            // (or to be integrated into mediation pipeline).
            services.AddValidatorsFromAssembly(typeof(ApplicationMarker).Assembly);
            services.AddScoped<IRequestHandler<RegisterUserCommand, Guid>, RegisterUserHandler>();



            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Mapster configuration
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(ApplicationMarker).Assembly); // optional but recommended

            services.AddSingleton(config);
            services.AddScoped<MapsterMapper.IMapper, Mapper>();
            services.AddScoped<Application.Mapping.IMapper, ServiceMapper>();

            // Central extension point to register Application-level services (CQRS, Mediator, Handlers, validators)
            services.AddApplicationServices(Configuration);

            // Infrastructure registration (EFCore, Repositories, Cache, Session stores, Email/SMS, etc.)
            services.AddInfrastructureServices(Configuration);

            // Authentication, Authorization, Session configuration
            services.AddAuthAndSession(Configuration);

            // HttpContextAccessor (common)
            services.AddHttpContextAccessor();

            // Other cross-cutting services (logging, health checks, etc.) can be registered here.
        }

        // Called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Standard pipeline with explicit calls to enable easy testing / extension
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Production error handling
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Session middleware (if configured by AddAuthAndSession)
            app.UseSession();

            // Custom middleware registrations go here (e.g. request logging, error mapping)
            // app.UseMiddleware<RequestLoggingMiddleware>();
            // app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                // Default MVC route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
