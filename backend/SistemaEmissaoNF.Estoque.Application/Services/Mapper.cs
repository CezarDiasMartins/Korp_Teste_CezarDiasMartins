using Mapster;
using SistemaEmissaoNF.Estoque.Application.Interfaces;

namespace SistemaEmissaoNF.Estoque.Application.Services;

public class Mapper(TypeAdapterConfig config) : IMapper
{
    public TDestination Map<TDestination>(object source)
    {
        return source.Adapt<TDestination>(config);
    }
}
