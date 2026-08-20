using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaEmissaoNF.Estoque.Application.UseCases.Estoque.Commands.Baixar;

namespace SistemaEmissaoNF.Estoque.Api.Controllers;

[ApiController]
[Route("api/estoque")]
public class EstoqueController(IMediator mediator) : ControllerBase
{
    [HttpPost("baixar")]
    public async Task<IActionResult> Baixar(BaixarEstoqueCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
