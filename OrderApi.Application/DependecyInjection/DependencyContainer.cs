using ECommmerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependecyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddAplicationservice
        (this IServiceCollection services, IConfiguration config)
    {
        //Registramos el Cliente Http
        //Creamos la injection de dependencia

        services.AddHttpClient<IOrderService,OrderSevice>(options =>
        {
            options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
            options.Timeout = TimeSpan.FromSeconds(1);
        });


        //Create Retry strategy 

        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(300),
            OnRetry = args =>
            {
                string message = $"OnRetry, Attemp:{args.AttemptNumber} OutCome {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebugger(message);
                return ValueTask.CompletedTask;
            }

        };

        // Usamos la estrategia de reintento
        services.AddResiliencePipeline("my-retry-pipeline", builder =>
        {
            builder.AddRetry(retryStrategy);
        });
        //Para propagar Header ebnn caso de las peticiones internas que necesiten Authorizacion
        services.AddHttpContextAccessor();
        return services;
    }
}
