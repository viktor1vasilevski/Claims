using Claims.Application.Channels;
using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Application.Validations.Claims;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<ICoversService, CoversService>();
        services.AddScoped<IAuditService, AuditService>();

        services.AddValidatorsFromAssemblyContaining<CreateClaimRequestValidator>(ServiceLifetime.Transient);

        services.AddSingleton<AuditChannel>();

        return services;
    }
}
