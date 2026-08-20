using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Infrastructure.Context;
using SistemaEmissaoNF.Faturamento.Infrastructure.Messaging;
using SistemaEmissaoNF.Faturamento.Infrastructure.Repositories;
using SistemaEmissaoNF.Faturamento.Infrastructure.Services;

namespace SistemaEmissaoNF.Faturamento.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFaturamentoInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool addWorkerConsumer = false)
    {
        services.AddDbContext<FaturamentoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("FaturamentoDb")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
        services.AddScoped<INotaFiscalPdfPublisher, NotaFiscalPdfPublisher>();
        services.AddScoped<INotaFiscalPdfService, NotaFiscalPdfService>();

        services.AddHttpClient<IEstoqueService, EstoqueService>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:EstoqueApi"] ?? "https://localhost:7001/");
            client.Timeout = TimeSpan.FromSeconds(configuration.GetValue("Services:EstoqueTimeoutSeconds", 5));
        });

        services.AddMassTransit(bus =>
        {
            if (addWorkerConsumer)
            {
                bus.AddConsumer<GerarNotaFiscalPdfConsumer>();
            }

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"] ?? "localhost", "/", host =>
                {
                    host.Username(configuration["RabbitMq:UserName"] ?? "guest");
                    host.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                if (addWorkerConsumer)
                {
                    cfg.ReceiveEndpoint("gerar-nota-fiscal-pdf", endpoint =>
                    {
                        endpoint.ConfigureConsumer<GerarNotaFiscalPdfConsumer>(context);
                    });
                }
            });
        });

        return services;
    }
}
