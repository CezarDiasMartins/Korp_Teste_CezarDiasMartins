using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Create;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Print;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.Get;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.GetPdf;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.List;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Api.Controllers;

[ApiController]
[Route("api/notas-fiscais")]
public class NotasFiscaisController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ListNotaFiscalQuery query, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(query, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetNotaFiscalQuery { Id = id }, cancellationToken);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateNotaFiscalCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("{id:int}/imprimir")]
    public async Task<IActionResult> Imprimir(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new PrintNotaFiscalCommand { Id = id }, cancellationToken);

        if (response.Success)
            return Ok(response);

        if (response.ServiceUnavailable)
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);

        return response.Errors.Any(x => x.Contains("não encontrada", StringComparison.OrdinalIgnoreCase))
            ? NotFound(response)
            : BadRequest(response);
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> GetPdf(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetNotaFiscalPdfQuery { Id = id }, cancellationToken);

        if (!response.Success || response.Data is null)
            return NotFound(response);

        if (response.Data.StatusImpressao == StatusImpressao.Concluido && response.Data.PdfArquivo is not null)
            return File(response.Data.PdfArquivo, "application/pdf", $"nota-fiscal-{id}.pdf");

        if (response.Data.StatusImpressao == StatusImpressao.Erro)
            return BadRequest(new GenericNoDataResponse { Errors = [response.Data.Message] });

        return Accepted(new GenericNoDataResponse { Errors = [response.Data.Message] });
    }
}
