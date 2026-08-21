using System.Net;
using SistemaEmissaoNF.Faturamento.Application.Response;

namespace SistemaEmissaoNF.Faturamento.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro não tratado na API de Faturamento.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new GenericNoDataResponse
            {
                Errors = ["Ocorreu um erro interno ao processar a requisição."]
            });
        }
    }
}
