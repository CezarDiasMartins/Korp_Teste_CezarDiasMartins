namespace SistemaEmissaoNF.Estoque.Application.Interfaces;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
}
