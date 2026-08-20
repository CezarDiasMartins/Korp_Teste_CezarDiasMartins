using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Infrastructure.Context;
using SistemaEmissaoNF.Estoque.Infrastructure.Repositories;

namespace SistemaEmissaoNF.Estoque.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEstoqueInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EstoqueDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("EstoqueDb")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProdutoRepository, ProdutoRepository>();

        return services;
    }
}
