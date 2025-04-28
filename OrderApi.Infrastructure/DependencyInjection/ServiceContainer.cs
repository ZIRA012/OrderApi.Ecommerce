using ECommmerce.SharedLibrary.DependencyInjection;
using ECommmerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection;

 public static class ServiceContainer
{

    public static IServiceCollection AddInfrastructureService(
        this IServiceCollection services, IConfiguration config)
    {
        //Agregamos ña coneccion de la base de datos 
        //la conexion 
        SharedServiceContainer.AddSharedServices<OrderDbContext>
            (services, config, config["MySerilog:FileName"]!);

        services.AddScoped<IOrder, OrderRepository>();
        return services;
    }

    public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
    {
        //Registramos los middlewares
        //GlobalException -> Handle External Error
        //Solo escuchar a ApiGateway 

        SharedServiceContainer.UseSharedPolicies(app);
        return app;
    }
}
