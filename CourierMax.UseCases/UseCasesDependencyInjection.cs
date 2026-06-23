using CourierMax.Dtos.Shipments;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CourierMax.UseCases;

public static class UseCasesDependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateShipmentValidator>();
        services.AddScoped<IValidator<CreateShipmentRequest>, CreateShipmentValidator>();

        services.AddScoped<CreateShipmentCommand>();
        services.AddScoped<ChangeShipmentStateCommand>();
        services.AddScoped<AssignShipmentCommand>();
        services.AddScoped<GetShipmentQuery>();
        services.AddScoped<GetDelayedShipmentsQuery>();
        services.AddScoped<GetDriverMetricsQuery>();

        services.AddScoped<ShipmentsUseCases>();
        services.AddScoped<DriversUseCases>();

        return services;
    }
}
