using System.Net;
using System.Net.Http.Json;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Services;

public class EstoqueService(HttpClient httpClient) : IEstoqueService
{
    public async Task<BaixaEstoqueResponse> BaixarEstoqueAsync(BaixaEstoqueRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var httpResponse = await httpClient.PostAsJsonAsync("api/estoque/baixar", request, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                return new BaixaEstoqueResponse();
            }

            var errors = await ReadErrorsAsync(httpResponse, cancellationToken);
            return new BaixaEstoqueResponse
            {
                Errors = errors.Count == 0 ? ["Nao foi possivel baixar o estoque."] : errors
            };
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ServiceUnavailableResponse();
        }
        catch (HttpRequestException)
        {
            return ServiceUnavailableResponse();
        }
    }

    private static BaixaEstoqueResponse ServiceUnavailableResponse()
    {
        return new BaixaEstoqueResponse
        {
            ServiceUnavailable = true,
            Errors =
            [
                "Nao foi possivel comunicar com o servico de estoque.",
                "A Nota Fiscal permanece aberta.",
                "Tente novamente."
            ]
        };
    }

    private static async Task<List<string>> ReadErrorsAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
    {
        if (httpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            return ServiceUnavailableResponse().Errors;
        }

        var response = await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
        return response?.Errors ?? [];
    }

    private class ErrorResponse
    {
        public List<string> Errors { get; set; } = [];
    }
}
