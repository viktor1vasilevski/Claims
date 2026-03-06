using System.Reflection;

namespace Claims.Api.Extensions;

/// <summary>
/// Provides extension methods for Swagger documentation configuration.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger documentation with XML comments to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}