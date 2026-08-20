namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
}
