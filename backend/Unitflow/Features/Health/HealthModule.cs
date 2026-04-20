namespace Unitflow.Features.Health;

public static class HealthModule
{
    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        return services;
    }
}
