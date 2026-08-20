using Mapster;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;

namespace SistemaEmissaoNF.Faturamento.Application.Services;

public class Mapper(TypeAdapterConfig config) : IMapper
{
    public TDestination Map<TDestination>(object source)
    {
        return source.Adapt<TDestination>(config);
    }
}
