using Scalar.AspNetCore;

namespace Unitflow.Infrastructure.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication UseOpenApiDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.MapOpenApi();
        app.MapScalarApiReference();
        return app;
    }
}
