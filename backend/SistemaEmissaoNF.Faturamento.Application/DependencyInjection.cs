using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Services;

namespace SistemaEmissaoNF.Faturamento.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFaturamentoApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, Mapper>();

        return services;
    }
}
