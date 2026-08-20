using MediatR;
using Microsoft.AspNetCore.Mvc;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Create;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Update;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Queries.Get;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Queries.List;

namespace SistemaEmissaoNF.Estoque.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ListProdutoQuery query, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(query, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProdutoQuery { Id = id }, cancellationToken);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateProdutoCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, UpdateProdutoCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await mediator.Send(command, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
