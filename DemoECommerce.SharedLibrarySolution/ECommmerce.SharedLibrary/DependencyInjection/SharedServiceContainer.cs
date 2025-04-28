using ECommmerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ECommmerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<Tcontext>
            (this IServiceCollection services, IConfiguration config, String filaName) where Tcontext : DbContext
        {
            //add generic Database Context 
            services.AddDbContext<Tcontext>(
                option => option.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"),
                sqlserverOption => sqlserverOption.EnableRetryOnFailure())
            );

            //configuramos para el log de serilog
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{filaName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{timestamp:yyyy-MM--dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj} {Newline} {Exception}",
                rollingInterval: RollingInterval.Day
                ).CreateLogger();


            //Add JWT authentication scheme

            JWTAuthenticationscheme.AddJWTAuthenticationScheme(services, config);
            return services;
        } 
    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //usar global exception
            app.UseMiddleware<GlobalException>();

            //registar middle ware para bloquear todo
            //app.UseMiddleware<ListeToOnlyApiGateway>();
            return app;
        }
    }
}
